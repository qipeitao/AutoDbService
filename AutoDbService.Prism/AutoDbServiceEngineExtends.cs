using AutoDbService.Models;
using AutoDbService.DbPrism.Interfaces;
using AutoDbService.DbPrism.Models;
using System;
using Prism.Mvvm;
using Prism.Modularity;
using Prism.Unity;
using System.Linq.Expressions;
using System.Windows;

namespace AutoDbService.DbPrism
{
    public static class AutoDbServiceEngineExtends
    {
        public static AutoDbServiceEngine UsePrism(this AutoDbServiceEngine engine)
        {
            engine.AddType<IInfoManagerViewModel<EntityBase>, InfoManagerViewModel<EntityBase>>(false);
            engine.AddType<IAddViewModel<EntityBase>, IAddViewModel<EntityBase>>(false);
            engine.AddType<IModifyViewModel<EntityBase>, IModifyViewModel<EntityBase>>(false);
            engine.AddType<IBuildDynamicType, BuildDynamicType>();
            engine.AddType<IPropertyAndCommandConvertName, PropertyAndCommandConvertName>();
            engine.AddType<IPrismModuleRegisterService, PrismModuleRegisterService>();
            engine.AddType<IModule, AutoDbModule>(false);

            return engine;
        }
        public static AutoDbServiceEngine ReConfigureViewModelLocator(this AutoDbServiceEngine engine)
        {
            ViewModelLocationProvider.SetDefaultViewModelFactory(
               t =>
               AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType(t)
               );
            ViewModelLocationProvider.SetDefaultViewModelFactory((view, t) =>
            AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType(t)
            ); 
            return engine;
        }
    }
}
