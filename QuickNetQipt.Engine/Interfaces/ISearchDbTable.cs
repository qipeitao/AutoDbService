using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNetQipt.Engine.Interfaces
{
    internal interface ISearchTable
    {
        List<Type> SearchTable(Type type);
    }
}
