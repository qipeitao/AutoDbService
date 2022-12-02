using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Asn1.X509.Qualified;
using QuickNetQipt.Engine.Dbs;
using QuickNetQipt.Engine.Interfaces;
using QuickNetQipt.Engine.Models;
using Type = System.Type;

namespace QuickNetQipt.Engine
{
    public class QuickNetQiptEngine : IDisposable
    {
        public readonly Type DbContextTypeKey = typeof(AutoMapContext);

        private Dictionary<Type, Tuple<Type, object>> serviceDics = new();
        private QuickNetQiptEngine()
        {

        }
        private static QuickNetQiptEngine quickNetQiptBulder;
        private static object lockQuickNetQiptBulder = new();
        public static QuickNetQiptEngine Instance
        {
            get
            {
                lock (lockQuickNetQiptBulder)
                {
                    if (quickNetQiptBulder == null)
                    {
                        quickNetQiptBulder = new QuickNetQiptEngine();
                    }
                }
                return quickNetQiptBulder;
            }
        }
         
        public QuickNetQiptEngine AddType(Type key, Type targetType, object obj, bool instance = true)
        {
            if (!serviceDics.ContainsKey(key))
            {
                serviceDics.Add(key, new Tuple<Type, object>(targetType, (instance ? (obj ?? CreateObjByType(targetType)) : null)));
            }
            return this;
        }
        public QuickNetQiptEngine AddType<IService, Service>(bool instance = true) where Service : class, IService
        {
            return AddType(typeof(IService), typeof(Service), null, instance);
        }
        public QuickNetQiptEngine AddTypeByValue(Type key, object obj, bool instance = true)
        {
            return AddType(key, obj.GetType(), obj, instance);
        }
        public QuickNetQiptEngine AddTypeByType<IService>(Type value, bool instance = true)
        {
            return AddType(typeof(IService), value, null, instance); ;
        }

        public void RemoveType(Type type)
        {
            if (serviceDics.ContainsKey(type))
            {
                serviceDics.Remove(type);
            }
        }
        public void RemoveType<IService>()
        {
            RemoveType(typeof(IService));
        }
        public void ReplaceServiceValue<IService>(IService v, bool instance = true)
        {
            if (serviceDics.ContainsKey(typeof(IService)))
                serviceDics[typeof(IService)] = new Tuple<Type, object>(v.GetType(), instance ? v : null);
        }
        public QuickNetQiptEngine SetDbSearchType<Service>() where Service : class, IDbTableSearch
        {
            return AddType<IDbTableSearch, Service>();
        }
        public QuickNetQiptEngine SetDbSetType<IService>() where IService : IDbService<object>
        {
            return AddTypeByType<IDbService<object>>(typeof(IService));
        }
        private QuickNetQiptEngine SetDbContext<TContext>() where TContext : AutoMapContext
        {
            return AddType<AutoMapContext, TContext>(false);
        }
        public QuickNetQiptEngine SetDbLinqFilter<T>() where T : IDbLinqFilter
        {
            return AddType<IDbLinqFilter, DbLinqFilter>();
        }
        public QuickNetQiptEngine UseAutoCreateDbService<TEntity, TContext>()
            where TContext : DbService<TEntity>
            where TEntity: class
        {
            return AddType<DbService<TEntity>, TContext>();
        }


        public void Builder<TContext>() where TContext : AutoMapContext
        {
            QuickNetQiptEngine.Instance.SetDbSearchType<DbTableSearch>();
            QuickNetQiptEngine.Instance.SetDbSetType<DbService<object>>();
            QuickNetQiptEngine.Instance.SetDbContext<TContext>();
            QuickNetQiptEngine.Instance.SetDbLinqFilter<DbLinqFilter>();
            QuickNetQiptEngine.Instance.AddType<IDbLinqInclude, DbLinqInclude>();
            QuickNetQiptEngine.Instance.AddType<IDbServiceCreator, DbServiceCreator>();

            Get<IDbTableSearch>().SearchTable(this[typeof(AutoMapContext)].GetType());
            var tableService = Get<IDbServiceCreator>().CreateDbService();
            tableService?.ToList().ForEach(p => { 
                AddTypeByValue(typeof(IDbService<>).MakeGenericType(p.Key), p.Value);
            });
        }
        /// <summary>
        /// 获取对象值
        /// </summary>
        /// <param name="type"></param>
        /// <returns>有实例即返回，无则新建</returns>
        public virtual object this[Type type]
        {
            get
            {
                if (serviceDics.ContainsKey(type))
                {
                    return serviceDics[type].Item2 ?? CreateObjByType(serviceDics[type].Item1);
                }
                else
                {
                    return null;
                }
            }
        }
        /// <summary>
        /// 获取数据库上下文，不存在则新建
        /// 没有注册，返回空
        /// </summary>
        public virtual AutoMapContext DbContext
        {
            get
            {
               return this[DbContextTypeKey] as AutoMapContext;
            }
        }

        public virtual IService Get<IService>()
        {
            return (IService)this[typeof(IService)];
        }
    
        private object CreateObjByType(Type type)
        {
            return Activator.CreateInstance(type);
        }

        public void Dispose()
        {
            serviceDics.Clear();
        }
    }
}