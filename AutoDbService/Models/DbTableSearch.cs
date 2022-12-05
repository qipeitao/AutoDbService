using AutoDbService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Models
{
    /// <summary>
    /// 搜索dbtable类型
    /// </summary>
    internal class DbTableSearch : IDbTableSearch
    { 
        public List<Type> DbTypes { private set; get; }= new List<Type>();
      

        /// <summary>
        /// 实现搜索
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Type> SearchTable(Type type)
        {
            if(DbTypes!=null&& DbTypes.Count!=0)return DbTypes;
            Assembly entityAssembly = type.Assembly;
            DbTypes = entityAssembly.GetTypes()
              .Where(p => IsMatch(type, p)).ToList();
            return DbTypes;
        }
        /// <summary>
        /// 匹配规则
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsMatch(Type context, Type type)
        {
            return !string.IsNullOrEmpty(type.Namespace) && type.Namespace == (context.Namespace + ".Entities");
        }
    }
}
