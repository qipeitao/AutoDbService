using QuickNetQipt.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNetQipt.Engine.Models
{
    public class DbServiceCreator : IDbServiceCreator
    {
        public Dictionary<Type, object> CreateDbService()
        {
            var tables = QuickNetQiptEngine.Instance.Get<IDbTableSearch>()?.DbTypes;
            if (tables == null || tables.Count == 0) return null;
            //////////////
            var dbServerObj = QuickNetQiptEngine.Instance.Get<IDbService<object>>();
            var baseType = dbServerObj.GetType().GetGenericTypeDefinition();
            var rs = tables.Select(p => {
                var t = baseType.MakeGenericType(p);
                return new { obj = Activator.CreateInstance(t), type = p };
            });
            return rs.ToDictionary(p=>p.type,p=>p.obj);
        }
    }
}
