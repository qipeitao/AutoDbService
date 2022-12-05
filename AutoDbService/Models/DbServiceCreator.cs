using AutoDbService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Models
{
    /// <summary>
    /// 默认服务创建器
    /// </summary>
    public class DbServiceCreator : IDbServiceCreator
    {
        /// <summary>
        /// 创建dbservice
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type, object> CreateDbService()
        {
            var tables = AutoDbServiceEngine.Instance.Get<IDbTableSearch>()?.DbTypes;
            if (tables == null || tables.Count == 0) return null;
            //////////////
            var dbServerObj = AutoDbServiceEngine.Instance.Get<IDbService<EntityBase>>();
            var baseType = dbServerObj.GetType().GetGenericTypeDefinition();
            var rs = tables.Select(p => {
                var t = baseType.MakeGenericType(p);
                return new { obj = Activator.CreateInstance(t), type = p };
            });
            return rs.ToDictionary(p=>p.type,p=>p.obj);
        }
    }
}
