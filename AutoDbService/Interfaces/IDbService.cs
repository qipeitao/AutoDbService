using AutoDbService.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Interfaces
{
    /// <summary>
    /// 基础服务接口
    /// </summary>
    public interface IDbService<TEntity> where TEntity : class
    {  
        /// <summary>
        /// 从数据库拿列表
        /// </summary>
        /// <param name="count">分页总条数</param>
        /// <param name="filter">筛选</param>
        /// <param name="orderBy">排序</param>
        /// <param name="funExtends">综合</param>
        /// <param name="enginePage">分表查询</param>
        /// <returns></returns>
        Task<List<TEntity>> GetListFromDb(out int count, Expression<Func<TEntity, bool>>? filter = default,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = default,
           Func<IQueryable<TEntity>, IQueryable<TEntity>>? funExtends = default, EnginePage enginePage = default);
        /// <summary>
        /// 从数据库中拿单
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        TEntity GetBaseOneFromDb(Expression<Func<TEntity, bool>>? filter = default);
        /// <summary>
        /// 新增 
        /// </summary>
        Boolean Add(TEntity entity);
        /// <summary>
        /// 删除 
        /// </summary>
        Boolean Remove(TEntity entity);
        /// <summary>
        /// 保存 
        /// </summary> 
        Boolean Save(TEntity entity);
        /// <summary>
        /// 根据数据实体进行填充
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="parmas"></param>
        /// <returns></returns>
        Task<TEntity> FillDetail(Expression<Func<TEntity, bool>>? filter, params object[] parmas);
    }
}
