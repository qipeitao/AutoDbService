using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickNetQipt.Lib.Dbs
{
    public class DbSetFinderFromAssmbly : IDbSetFinder
    {
        private readonly ConcurrentDictionary<Type, IReadOnlyList<DbSetProperty>> _cache
            = new ConcurrentDictionary<Type, IReadOnlyList<DbSetProperty>>();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IReadOnlyList<DbSetProperty> FindSets(Type contextType)
            => _cache.GetOrAdd(contextType, FindTypesNonCached);
        private static DbSetProperty[] FindTypesNonCached(Type contextType)
        {
            var assembly = contextType.Assembly;
            var newSpace = (contextType.Namespace + ".Entities").ToLower();
            var types = assembly.GetTypes().Where(p =>
            p.Namespace.ToLower() == newSpace && p.IsClass).ToList();

            var factory = new ClrPropertySetterFactory();
            return types.OrderBy(p => p.Name).Select(p => new DbSetProperty(
                        p.Name,
                        p,
                        null)).ToArray();
        }
    }
}
