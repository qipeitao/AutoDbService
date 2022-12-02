using QuickNetQipt.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNetQipt.Engine.Extends
{
    public static class DbLinqIncludeExtends
    {
        /// <summary>
        /// 调用IDbLinqInclude 自动填充Include
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public static IQueryable<TEntity> AutoInclude<TEntity>(this IQueryable<TEntity> entities) where TEntity : class
        {
            var db = QuickNetQiptEngine.Instance.Get<IDbLinqInclude>();
            if (db == null) return entities;
             return db.AutoInclude(entities);
        }
    }
}
