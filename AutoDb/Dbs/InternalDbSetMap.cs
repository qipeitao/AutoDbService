using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using AutoDbService.Extends;
using AutoDbService.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Dbs
{
    /// <summary>
    /// 实现automap的的dbset
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class InternalDbSetMap<TEntity> : InternalDbSet<TEntity>
        where TEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entityTypeName"></param>
        public InternalDbSetMap([NotNull] DbContext context, string entityTypeName) : base(context, entityTypeName)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Add(TEntity m)
        {
            var entity = m.Copy();
            return base.Add(entity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<EntityEntry<TEntity>> AddAsync(
            TEntity m,
            CancellationToken cancellationToken = default)
        {
            var entity = m.Copy();
            return base.AddAsync(entity, cancellationToken);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Attach(TEntity m)
        {
            var entity = m.Copy();
            return base.Attach(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Remove(TEntity m)
        {
            var entity = m.Copy();
            return base.Remove(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Update(TEntity m)
        {
            var entity = m.Copy();
            return base.Update(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void AddRange(params TEntity[] m)
        {
            var entities = m.ToList().Copy();
            base.AddRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <returns></returns>
        public override Task AddRangeAsync(params TEntity[] m)
        {
            var entities = m.ToList().Copy();
            return base.AddRangeAsync(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void AttachRange(params TEntity[] m)
        {
            var entities = m.ToList().Copy();
            base.AttachRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void RemoveRange(params TEntity[] m)
        {
            var entities = m.ToList().Copy();
            base.RemoveRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void UpdateRange(params TEntity[] m)
        {
            var entities = m.ToList().Copy();
            base.UpdateRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void AddRange(IEnumerable<TEntity> m)
        {
            var entities = m.ToList().Copy();
            base.AddRange(entities);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task AddRangeAsync(
            IEnumerable<TEntity> m,
            CancellationToken cancellationToken = default)
        {
            var entities = m.ToList().Copy();
            return base.AddRangeAsync(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void AttachRange(IEnumerable<TEntity> m)
        {
            var entities = m.ToList().Copy();
            base.AttachRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void RemoveRange(IEnumerable<TEntity> m)
        {
            var entities = m.ToList().Copy();
            base.RemoveRange(entities);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="m"></param>
        public override void UpdateRange(IEnumerable<TEntity> m)
        {
            var entities = m.ToList().Copy();
            base.UpdateRange(entities);
        }
    }
}
