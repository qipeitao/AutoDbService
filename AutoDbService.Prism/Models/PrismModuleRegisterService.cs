using AutoDbService.DbPrism.Attributes;
using AutoDbService.DbPrism.Interfaces;
using AutoDbService.Models;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Unity;
using AutoDbService.DbPrism.Extends;

namespace AutoDbService.DbPrism.Models
{
    public class PrismModuleRegisterService : IPrismModuleRegisterService
    {
        public void DbRegisterTypes(IContainerRegistry containerRegistry, IModule obj)
        {
            IUnityContainer unityContainer = CommonServiceLocator.ServiceLocator.Current.GetInstance<IUnityContainer>();
            RegisterModule(containerRegistry,obj.GetType(), unityContainer);
        }
        public void RegisterModule(IContainerRegistry containerRegistry, Type type, IUnityContainer unityContainer)
        {
            var types = type.Assembly.GetExportedTypes().ToList();
            types.ForEach(p =>
            {
                var manager = p.GetCustomAttribute<DbTableManagerViewAttribute>();
                if (manager != null)
                {
                    var createType = GetViewModelByViewType<IInfoManagerViewModel<EntityBase>>(p, manager.TableType, types);
                    unityContainer.RegisterFactory(createType, createType.FullName,
                        (u,t,s) => AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType(t),null);
                    containerRegistry.RegisterForNavigationWithViewModel(p, createType);
                    return;
                }
                var add = p.GetCustomAttribute<DbTableAddViewAttribute>();
                if (add != null)
                {
                    var createType = GetViewModelByViewType<IAddViewModel<EntityBase>>(p, add.TableType, types);
                    unityContainer.RegisterFactory(createType, createType.FullName,
                        (u, t, s) => AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType(t), null);
                    containerRegistry.RegisterForNavigationWithViewModel(p, createType); 
                    return;
                }
                var modify = p.GetCustomAttribute<DbTableModifyViewAttribute>();
                if (modify != null)
                {
                    var createType = GetViewModelByViewType<IModifyViewModel<EntityBase>>(p, modify.TableType, types);
                    unityContainer.RegisterFactory(createType, createType.FullName,
                        (u, t, s) => AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType(t), null);
                    containerRegistry.RegisterForNavigationWithViewModel(p, createType);   
                    return;
                }
            });
        }
        private Type GetViewModelByViewType<TType>(Type view, Type enityType, List<Type> types)
        {
            string modelName = GetViewModelNameByViewType(view); 
            var type= types.FirstOrDefault(p => p.Name == modelName);
            if (type != null) return type;
            ////////////////////////////////////
            var copyType = AutoDbServiceEngine.Instance.GetType(typeof(TType));
            if(copyType!=null)
            {
              return  copyType.GetGenericTypeDefinition().MakeGenericType(enityType);
            }
            return null;
        }

        private string  GetViewModelNameByViewType(Type view)
        {
            string modelName = String.Empty;
            if (view.Name.EndsWith("ViewModel"))
            {
                modelName = view.Name;
            }
            else if (view.Name.EndsWith("View"))
            {
                modelName = view.Name + "Model";
            }
            else
            {
                modelName = view.Name + "ViewModel";
            }
            return modelName;
        }
    }
}
