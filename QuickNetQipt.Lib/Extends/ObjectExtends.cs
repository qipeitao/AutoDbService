using System;
using System.Collections;
using System.Collections.Generic;
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
        public static T Map<T>(this T t)
        {
            return Copy<T>(t);
        }
        public static T Copy<T>(this T source, int level = 0)
        {
            if (source == null)
            {
                return default(T);
            }
            if (source is IEnumerable<object> objs)
            {
                if (objs.Count() == 0) return default;
                var items = objs.ToArray().Select(p => ObjectToClone(p, 0, level)).ToList();
                if (items is T t)
                {
                    return t;
                }
                else
                {
                    return default;
                }

            }
            else
            {
                return ObjectToClone(source, 0, level);
            }
        }
        private static T ObjectToClone<T>(T source, int level, int maxLevel)
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
                        var v = p.GetValue(source);
                        if (v is IEnumerable vv
                        ||p.PropertyType.GetRuntimeProperties().Any(t=>t.GetCustomAttribute<KeyAttribute>()!=null))//列表类
                        {

                        }
                        else
                        {
                            p.SetValue(result, v);
                        }
                    }
                    else
                    {
                        if (level < maxLevel)
                        {
                            var newv = ObjectToClone(p.GetValue(source), level + 1, maxLevel);
                            p.SetValue(result, newv);
                        }
                    }
                });

            return (T)result;
        }
    }
}
