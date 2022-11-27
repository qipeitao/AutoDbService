using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace QuickNetQipt.Lib.Extends
{
    public static class ObjectExtends
    {
        /// <summary>
        /// 复制一份
        /// 对JsonIgnoreAttribute 追加深度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="level"></param>
        /// <param name="maxLevel">最大深度</param>
        /// <param name="level">当前深度，当前深度达到最大深度返回当前值</param>
        /// <returns></returns>
        public static T Copy<T>(this T source, int level = 0, int maxLevel = 1) where T : class
        {
            if (source == null)
            {
                return default(T);
            }
            if (level >= maxLevel)
            {
                return source;
            }
            if (source is ICollection)
            {
                var t = source.GetType().Assembly.CreateInstance(source.GetType().FullName);
                var items = ((ICollection)source).Cast<object>()
                   .Select(p => p.ObjectToClone(level + 1, maxLevel)).ToArray();//
                var methods = t.GetType().GetRuntimeMethods();
                var add = methods.FirstOrDefault(m => m.Name == nameof(Collection<T>.Add));
                if (add != null)
                {
                    items.ToList().ForEach(p => add.Invoke(t, new object[] { p }));
                    return t as T;
                }
                return t as T;
            }
            return source.ObjectToClone(level + 1, maxLevel);
        }
        private static T ObjectToClone<T>(this T source, int level, int maxLevel) where T : class
        {
            if (source == null) return default(T);
            var type = source.GetType();
            var result = type.Assembly.CreateInstance(type.FullName);
            source.GetType().GetProperties()
                .ToList().ForEach(p =>
                {
                    /////////普通属性
                    if (p.CanWrite
                    && p.GetCustomAttribute<ForeignKeyAttribute>() == null
                    && p.GetCustomAttribute<NotMappedAttribute>() == null
                    && p.GetCustomAttribute<JsonIgnoreAttribute>() == null)
                    {
                        var sobj = p.GetValue(source);
                        p.SetValue(result, sobj);
                    }
                    else if (p.CanWrite
                    && (p.GetCustomAttribute<ForeignKeyAttribute>() == null
                    || p.GetCustomAttribute<NotMappedAttribute>() == null
                    || p.GetCustomAttribute<JsonIgnoreAttribute>() == null))
                    {
                        if (level < maxLevel)
                        {
                            var sobj = p.GetValue(source).Copy(level + 1, maxLevel);
                            p.SetValue(result, sobj);
                        }
                    }
                    else
                    {

                    }
                });

            return (T)result;
        }
    }
}
