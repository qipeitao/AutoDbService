using AutoDbService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Models
{
    public class DbServiceCreator : IDbServiceCreator
    {
        public Dictionary<Type, object> CreateDbService()
        {
            var tables = AutoDbServiceEngine.Instance.Get<IDbTableSearch>()?.DbTypes;
            if (tables == null || tables.Count == 0) return null;
            //////////////
            var dbServerObj = AutoDbServiceEngine.Instance.Get<IDbService<object>>();
            var baseType = dbServerObj.GetType().GetGenericTypeDefinition();
            var rs = tables.Select(p => {
                var t = baseType.MakeGenericType(p);
                return new { obj = Activator.CreateInstance(t), type = p };
            });
            return rs.ToDictionary(p=>p.type,p=>p.obj);
        }
    }
}
