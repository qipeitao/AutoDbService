using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Dbs
{
    /// <summary>
    /// 自定义创建参数
    /// </summary>
    public class CustmDbContextOptionsExtension : IDbContextOptionsExtension
    {
        private DbContextOptionsExtensionInfo _info;
        public DbContextOptionsExtensionInfo Info
            => _info ??= new ExtensionInfo(this);

        private Type _intefaceType;
        private Type _typeObj;
        public CustmDbContextOptionsExtension(Type intefaceType, Type typeObj)
        {
            _intefaceType = intefaceType;
            _typeObj = typeObj;
        }
        public virtual void ApplyServices(IServiceCollection services)
            => services.Add(new ServiceDescriptor(_intefaceType, _typeObj.Assembly.CreateInstance(_typeObj.FullName)));

        public virtual void Validate(IDbContextOptions options)
        {
        }

        private sealed class ExtensionInfo : DbContextOptionsExtensionInfo
        {
            public ExtensionInfo(IDbContextOptionsExtension extension)
                : base(extension)
            {
            }

            public override bool IsDatabaseProvider
                => false;

            //public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            //    => true;

            public override string LogFragment
                => "";

            public override int GetServiceProviderHashCode()
            {
                return 0;
            }

            public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
            {
            }

            public override bool ShouldUseSameServiceProvider(DbContextOptionsExtensionInfo other)
            {
                return true;
            }
        }
    }
}
