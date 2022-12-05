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
using AutoDbService.Interfaces;

namespace AutoDbService.Dbs
{
    /// <summary>
    /// 创建于DbContext
    /// 1. 自动搜集数据库表注册;也可以显式指定方式，使用此方法需要注销目前自动化实现（OnModelCreating）。
    /// 2. 重写普通操作，采用自定义的dbset，支持自动map
    /// </summary>
    public class AutoMapContext : DbContext
    {
        /// <summary>
        /// 创建启动配置
        /// </summary>
        /// <returns></returns>
        public static DbContextOptionsBuilder DbContextOptionsBuilder()
        {
           return new DbContextOptionsBuilder()
                     .AddCustTypes<IDbSetSource, DbSetSourceMap>(); 
        }
        /// <summary>
        /// 
        /// </summary>
        public AutoMapContext() : base(DbContextOptionsBuilder().Options)
        {
            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public AutoMapContext([NotNull] DbContextOptions options) : base(options)
        {
            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        } 
         /// <summary>
         /// 自动实现库表注册
         /// </summary>
         /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            var tables = AutoDbServiceEngine.Instance.Get<IDbTableSearch>().DbTypes; 
            foreach (Type type in tables)
            {
                dynamic configurationInstance = Activator.CreateInstance(type);
                if (modelBuilder.Model.FindEntityType(type.FullName) == null)
                {
                    modelBuilder.Model.AddEntityType(type);
                }
            } 
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry Add(object entity)
        {
            return base.Add(entity.Copy());
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            return base.Add(entity.Copy());
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<EntityEntry> AddAsync(object entity, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(entity.Copy(), cancellationToken);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entities"></param>
        public override void AddRange(IEnumerable<object> entities)
        {
            var items = entities.ToList().Copy();
            if (items == null) return;
            base.AddRange(items);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entities"></param>
        public override void AddRange(params object[] entities)
        {
            AddRange(entities.ToList());
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override Task AddRangeAsync(IEnumerable<object> entities, CancellationToken cancellationToken = default)
        {
            return base.AddRangeAsync(entities.Copy(), cancellationToken);
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        public override Task AddRangeAsync(params object[] entities)
        {
            return base.AddRangeAsync(entities.ToList().Copy());
        }
        /// <summary>
        /// 新增
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public override ValueTask<EntityEntry<TEntity>> AddAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default)
        {
            return base.AddAsync(entity.Copy(), cancellationToken);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry Update(object entity)
        {
            return base.Update(entity.Copy());
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            return base.Update(entity.Copy());
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entities"></param>
        public override void UpdateRange(IEnumerable<object> entities)
        {
            var items = entities.ToList().Copy();
            if (items == null) return;
            base.UpdateRange(items);
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="entities"></param>
        public override void UpdateRange(params object[] entities)
        {
            UpdateRange(entities.ToList());
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry Remove(object entity)
        {
            return base.Remove(entity.Copy());
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            return base.Remove(entity.Copy());
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entities"></param>
        public override void RemoveRange(IEnumerable<object> entities)
        {
            var items = entities.ToList().Copy();
            if (items == null) return;
            base.RemoveRange(items);
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="entities"></param>
        public override void RemoveRange(params object[] entities)
        {
            RemoveRange(entities.ToList());
        }
    }
}
