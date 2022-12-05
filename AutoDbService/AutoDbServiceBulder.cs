using AutoDbService.Dbs;
using AutoDbService.Interfaces;
using AutoDbService.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore.Internal;
using Org.BouncyCastle.Asn1.X509.Qualified; 
using Type = System.Type;

namespace AutoDbService
{
    /// <summary>
    /// 服务编辑器
    /// </summary>
    public class AutoDbServiceEngine : IDisposable
    {
        public readonly Type DbContextTypeKey = typeof(AutoMapContext);

        private Dictionary<Type, Tuple<Type, object>> serviceDics = new();
        private AutoDbServiceEngine()
        {

        }
        private static AutoDbServiceEngine AutoDbServiceBulder;
        private static object lockAutoDbServiceBulder = new();
        public static AutoDbServiceEngine Instance
        {
            get
            {
                lock (lockAutoDbServiceBulder)
                {
                    if (AutoDbServiceBulder == null)
                    {
                        AutoDbServiceBulder = new AutoDbServiceEngine();
                    }
                }
                return AutoDbServiceBulder;
            }
        }
        /// <summary>
        /// 新增类型
        /// </summary>
        /// <param name="key">关键字</param>
        /// <param name="targetType">目标类型</param>
        /// <param name="obj">实例</param>
        /// <param name="instance">是否单例。单例：自动生成一份，其他随用随生成</param>
        /// <returns></returns>
        public AutoDbServiceEngine AddType([NotNull] Type key, [NotNull] Type targetType, object obj, bool instance = true)
        {
            if (!serviceDics.ContainsKey(key))
            {
                serviceDics.Add(key, new Tuple<Type, object>(targetType, (instance ? (obj ?? CreateObjByType(targetType)) : null)));
            }
            return this;
        }
        private AutoDbServiceEngine AddType<IService, Service>(bool instance = true) where Service : class, IService
        {
            return AddType(typeof(IService), typeof(Service), null, instance);
        }
        private AutoDbServiceEngine AddTypeByValue([NotNull] Type key, object obj, bool instance = true)
        {
            return AddType(key, obj.GetType(), obj, instance);
        }
        private AutoDbServiceEngine AddTypeByType<IService>(Type value, bool instance = true)
        {
            return AddType(typeof(IService), value, null, instance); ;
        }
        /// <summary>
        /// 删除类型 
        /// </summary>
        /// <param name="type"></param>
        public void RemoveType([NotNull] Type type)
        {
            if (serviceDics.ContainsKey(type))
            {
                serviceDics.Remove(type);
            }
        }
        /// <summary>
        /// 删除类型
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        public void RemoveType<IService>()
        {
            RemoveType(typeof(IService));
        }
        /// <summary>
        /// 替换服务
        /// </summary>
        /// <typeparam name="IService">被替换的服务类型</typeparam>
        /// <param name="v">替换的服务实例,不能为空</param>
        /// <param name="instance"></param>
        public bool ReplaceServiceValue<IService>([NotNull]IService v, bool instance = true)
        {
            if(v==null)
            {
                return false;
            }
            if (serviceDics.ContainsKey(typeof(IService)))
            {
                serviceDics[typeof(IService)] = new Tuple<Type, object>(v.GetType(), instance ? v : null);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 设置搜寻数据实体服务
        /// </summary>
        /// <typeparam name="Service"></typeparam>
        /// <returns></returns>
        public AutoDbServiceEngine SetDbSearchType<Service>() where Service : class, IDbTableSearch
        {
            return AddType<IDbTableSearch, Service>();
        }
        /// <summary>
        /// 设置内置dbservice服务基类
        /// 引擎将用此基类进行服务创建
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <returns></returns>
        public AutoDbServiceEngine SetDbServiceType<IService>() where IService : IDbService<object>
        {
            return AddTypeByType<IDbService<object>>(typeof(IService));
        }
        /// <summary>
        /// 设置数据库内容上下文
        /// </summary>
        /// <typeparam name="TContext">需继承与AutoMapContext</typeparam>
        /// <returns></returns>
        private AutoDbServiceEngine SetDbContext<TContext>() where TContext : AutoMapContext
        {
            return AddType<AutoMapContext, TContext>(false);
        }
     /// <summary>
     /// 创建构建环境
     /// </summary>
     /// <typeparam name="TContext"></typeparam>
        public void Builder<TContext>() where TContext : AutoMapContext
        {
            AutoDbServiceEngine.Instance.SetDbSearchType<DbTableSearch>();
            AutoDbServiceEngine.Instance.SetDbServiceType<DbService<object>>();
            AutoDbServiceEngine.Instance.SetDbContext<TContext>(); 
            AutoDbServiceEngine.Instance.AddType<IDbLinqFilter, DbLinqFilter>();
            AutoDbServiceEngine.Instance.AddType<IDbLinqInclude, DbLinqInclude>();
            AutoDbServiceEngine.Instance.AddType<IDbServiceCreator, DbServiceCreator>();

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
        /// <summary>
        /// 获取服务对应的实例
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <returns></returns>
        public virtual IService Get<IService>()
        {
            return (IService)this[typeof(IService)];
        }
    /// <summary>
    /// 是否注册类型
    /// </summary>
    /// <typeparam name="IService"></typeparam>
    /// <returns></returns>
        public virtual bool IsRegister<IService>()
        {
            return IsRegister(typeof(IService));
        }
        /// <summary>
        /// 是否注册类型
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool IsRegister(Type type)
        {
            return serviceDics.ContainsKey(type);
        }
        /// <summary>
        /// 是否拥有实例
        /// </summary>
        /// <typeparam name="IService"></typeparam>
        /// <returns></returns>
        public virtual bool HasInstance<IService>()
        {
            return HasInstance(typeof(IService));
        }
        /// <summary>
        /// 是否拥有实例
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public virtual bool HasInstance(Type type)
        {
            if (serviceDics.ContainsKey(type))
            {
                return serviceDics[type].Item2 !=null;
            }
            else
            {
                return false;
            }
        }
        private object CreateObjByType(Type type)
        {
            return Activator.CreateInstance(type);
        }
        /// <summary>
        /// 资源释放
        /// </summary>
        public void Dispose()
        {
            serviceDics.Clear();
        }
    }
}