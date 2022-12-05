using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Internal;
using AutoDbService.Extends;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Dbs
{ 
    public class AutoMapContext : DbContext
    {  
        public static DbContextOptionsBuilder DbContextOptionsBuilder()
        {
           return new DbContextOptionsBuilder()
                     .AddCustTypes<IDbSetSource, DbSetSourceMap>(); 
        }
        public AutoMapContext() : base(DbContextOptionsBuilder().Options)
        {
            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        }
        public AutoMapContext([NotNull] DbContextOptions options) : base(options)
        {
            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        } 
         
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Assembly entityAssembly = this.GetType().Assembly;
            IEnumerable<Type> typesToRegister = entityAssembly.GetTypes()
                .Where(p => !string.IsNullOrEmpty(p.Namespace) && p.Namespace == "AutoDbService.Entity.Entities");
            foreach (Type type in typesToRegister)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                if (modelBuilder.Model.FindEntityType(type.FullName) == null)
                {
                    modelBuilder.Model.AddEntityType(type);
                }
            } 
        }


        public override EntityEntry Add(object entity)
        {
            return base.Add(entity.Copy());
        }
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            return base.Add(entity.Copy());
        }
        public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(entity.Copy(), cancellationToken);
        }
        public override void AddRange(IEnumerable<object> entities)
        {
            var items = entities.ToList().Copy();
            if (items == null) return;
            base.AddRange(items);
        }
        public override void AddRange(params object[] entities)
        {
            AddRange(entities.ToList());
        }
        public override Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            return base.AddRangeAsync(entities.Copy(), cancellationToken);
        }
        public override Task AddRangeAsync(params object[] entities)
        {
            return base.AddRangeAsync(entities.ToList().Copy());
        }
        public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(entity.Copy(), cancellationToken);
        } 
        public override EntityEntry Update(object entity)
        {
            return base.Update(entity.Copy());
        }
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            return base.Update(entity.Copy());
        } 
        public override void UpdateRange(IEnumerable<object> entities)
        {
            var items = entities.ToList().Copy();
            if (items == null) return;
            base.UpdateRange(items);
        }
        public override void UpdateRange(params object[] entities)
        {
            UpdateRange(entities.ToList());
        }

        public override EntityEntry Remove(object entity)
        {
            return base.Remove(entity.Copy());
        }
        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            return base.Remove(entity.Copy());
        }
        public override void RemoveRange(IEnumerable<object> entities)
        {
            var items = entities.ToList().Copy();
            if (items == null) return;
            base.RemoveRange(items);
        }
        public override void RemoveRange(params object[] entities)
        {
            RemoveRange(entities.ToList());
        }
    }
}
