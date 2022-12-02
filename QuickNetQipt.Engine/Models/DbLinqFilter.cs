using QuickNetQipt.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using System.Collections;
using System.Data;
using QuickNetQipt.Engine.Utilities;

namespace QuickNetQipt.Engine.Models
{
    /// <summary>
    /// 自动生成过滤规则
    /// </summary>
    public class DbLinqFilter : IDbLinqFilter
    {
        public Expression<Func<TEntity, bool>>? GetDbLinqFilter<TEntity>()
        {
            throw new NotImplementedException();
        } 
      
    }
    public  class DbLinqInclude : IDbLinqInclude
    { 
        public IQueryable<TEntity> AutoInclude<TEntity>(IQueryable<TEntity> entities) where TEntity : class
        {
            var ins = AutoIncludeProperty<TEntity>();
            ins?.ForEach(p =>
            {
                entities = entities.Include(p);
            });
            return entities;
        }
        private List<Expression<Func<TEntity, object>>> AutoIncludeProperty<TEntity>()
        {
            IDbTableSearch table = QuickNetQiptEngine.Instance.Get<IDbTableSearch>();
            ParameterExpression source = Expression.Parameter(typeof(TEntity));
            var sourceProps = typeof(TEntity).GetProperties(BindingFlags.Instance | BindingFlags.Public).Where(p => p.CanWrite).ToList();
            // 查找可进行关联的属性
            //p.PropertyType.GenericTypeArguments.Single()
            var copyProps = sourceProps.Where(tProp => table.DbTypes.Contains(tProp.PropertyType)
            || (
            tProp.PropertyType.GetTypeInfo().IsGenericType
             && tProp.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
             || (
            tProp.PropertyType.GetTypeInfo().IsGenericType
             && tProp.PropertyType.GetGenericTypeDefinition() == typeof(ObservableCollection<>))
             || (
            tProp.PropertyType.GetTypeInfo().IsGenericType
             && tProp.PropertyType.GetGenericTypeDefinition() == typeof(IList<>)));
            //////////////////////////////////////////////
           var result= copyProps.Select(p => Expression.Lambda<Func<TEntity, object>>(Expression.Property(source, p.Name), source)).ToList(); 
            return result; 
        }
         
    }  

}
