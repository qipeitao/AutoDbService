using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Interfaces
{
    public interface IDbTableSearch
    {
        List<Type> DbTypes { get; }
        List<Type> SearchTable(Type type);
        bool IsMatch(Type context, Type type);
    }
}
