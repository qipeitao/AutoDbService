using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using AutoDbService.Dbs;
using AutoDbService.Extends;
using AutoDbService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AutoDbService.Models
{
    /// <summary>
    /// 实现dbset的服务基类
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public class DbService<TEntity> : IDbService<TEntity> where TEntity : EntityBase
    { 
        /// <summary>
        /// 排序规则
        /// </summary>
        protected Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> _orderBy;
        /// <summary>
        /// 内部处理规则
        /// </summary>
        protected Func<IQueryable<TEntity>, IQueryable<TEntity>> _funExtends;
        /// <summary>
        /// 
        /// </summary> 
        /// <param name="orderBy">排序规则</param>
        /// <param name="_funExtends">内部处理规则,可添加include</param>
        public DbService(
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
            Func<IQueryable<TEntity>, IQueryable<TEntity>>? funExtends)
        { 
            this._orderBy = orderBy;
            this._funExtends = funExtends;
        }
        /// <summary>
        /// 采用默认方法构建
        /// </summary>
        public DbService()
        {
            this._funExtends = s => s.AutoInclude();
        }
         
        #region Get 
        /// <summary>
        /// 查询一个
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>

        public virtual TEntity GetBaseOneFromDb(Expression<Func<TEntity, bool>> filter = default)
        {
            using (var db = AutoDbServiceEngine.Instance.DbContext)
            {
                return db.Set<TEntity>().Where(filter).FirstOrDefault();
            }
        }
        #endregion 
        /// <summary>
        /// 查询多个
        /// </summary>
        /// <param name="count">全部条数</param>
        /// <param name="filter">过滤条件</param>
        /// <param name="orderBy">排序条件</param>
        /// <param name="funExtends">扩展方法(默认采用自动include)</param>
        /// <param name="enginePage">分页（最小每页5条数据）</param>
        /// <returns></returns>
        public virtual Task<List<TEntity>> GetListFromDb(out int count, Expression<Func<TEntity, bool>>? filter = default,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = default,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> funExtends = default, EnginePage enginePage = default)
        {
            using (var db = AutoDbServiceEngine.Instance.DbContext)
            {
                IQueryable<TEntity> query = db.Set<TEntity>();
                if (filter != default)
                    query = query.Where(filter);
                count = query.Count();
                if (orderBy != default)
                {
                    query = orderBy(query);
                }
                else if(_orderBy!=default)
                {
                    query = _orderBy(query);
                } 
                if (enginePage != null)
                {
                    var skip = enginePage.Skip < 0 ? 0 : enginePage.Skip;
                    var take = enginePage.Take < 5 ? 5 : enginePage.Take;
                    query = query.Skip(skip).Take(take);
                }
                if (funExtends != default)
                {
                    query = funExtends(query);
                }
                else if (_funExtends != default)
                {
                    query = _funExtends(query);
                }
                return query.ToListAsync();
            }
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Add(TEntity obj)
        {
            if (obj == null) return false;
            using (var db = AutoDbServiceEngine.Instance.DbContext)
            {
                db.Add(obj);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual bool Remove(TEntity obj)
        {
            if (obj == null) return false;
            using (var db = AutoDbServiceEngine.Instance.DbContext)
            {
                db.Remove(obj);
                return db.SaveChanges() > 0;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public virtual Boolean Save(TEntity obj)
        {
            if (obj == null) return false;
            using (var db = AutoDbServiceEngine.Instance.DbContext)
            {
                var entity = db.Update<TEntity>(obj);
                return db.SaveChanges() > 0;
            }
        }

        /// <summary>
        /// 填充model
        /// 采用默认扩展_funExtends
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="parmas"></param>
        public virtual async Task<TEntity> FillDetail(Expression<Func<TEntity, bool>>? filter, params object[] parmas)
        {
            var list = await GetListFromDb(out int count, filter, null, _funExtends);
            return list.FirstOrDefault();
        }
    }
}
