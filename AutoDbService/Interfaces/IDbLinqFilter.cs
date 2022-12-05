using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.Interfaces
{
    public interface IDbLinqFilter
    {
        Expression<Func<TEntity, bool>>? GetDbLinqFilter<TEntity>();
    }
    public interface IDbLinqInclude
    {
        IQueryable<TEntity> AutoInclude<TEntity>(IQueryable<TEntity> entities) where TEntity : class; 
    }
}
