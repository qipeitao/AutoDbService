using QuickNetQipt.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace QuickNetQipt.Engine.Models
{
    /// <summary>
    /// 搜索dbtable类型
    /// </summary>
    internal class SearchDbTable : ISearchTable
    {
        /// <summary>
        /// 实现搜索
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<Type> SearchTable(Type type)
        {
            Assembly entityAssembly = this.GetType().Assembly;
              return   entityAssembly.GetTypes()
                .Where(p => IsMatch(type,p)).ToList(); 
        }
        /// <summary>
        /// 匹配规则
        /// </summary>
        /// <param name="context"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsMatch(Type context,Type type)
        {
            return !string.IsNullOrEmpty(type.Namespace) && type.Namespace == (context.Namespace+".Entities");
        }
    }
}
