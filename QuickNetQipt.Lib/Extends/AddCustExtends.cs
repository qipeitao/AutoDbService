using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using QuickNetQipt.Lib.Dbs;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NotNullAttribute = QuickNetQipt.Lib.Dbs.NotNullAttribute;

namespace QuickNetQipt.Lib.Extends
{
    public static class AddCustExtends
    {
        public static DbContextOptionsBuilder AddCustTypes<IType, Type>([NotNull] this DbContextOptionsBuilder builder)
            where Type : IType, new()
        {
            var extension = new CustmDbContextOptionsExtension(typeof(IType), typeof(Type));
            ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
            return builder;
        }
    }
}
