using AutoDbService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Interfaces
{
    /// <summary>
    /// 通用查询条件
    /// </summary>
    public interface IDbLinqFilter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        Expression<Func<TEntity, bool>>? GetDbLinqFilter<TEntity>() where TEntity : EntityBase; 
    }
    /// <summary>
    /// 通用默认include实现
    /// </summary>
    public interface IDbLinqInclude
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        IQueryable<TEntity> AutoInclude<TEntity>(IQueryable<TEntity> entities) where TEntity : EntityBase; 
    }
}
