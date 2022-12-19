using Prism.Ioc;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Extends
{
    public static class IContainerRegistryExtensions
    { 
        /// <summary>
        /// 扩展注册vm
        /// </summary>
        /// <param name="containerRegistry"></param>
        /// <param name="viewType"></param>
        /// <param name="viewModelType"></param>
        /// <param name="name"></param>
        public static void RegisterForNavigationWithViewModel(this IContainerRegistry containerRegistry, Type viewType, Type viewModelType,string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                name = viewType.Name;
            } 
            ViewModelLocationProvider.Register(viewType.ToString(), viewModelType);
            containerRegistry.RegisterForNavigation(viewType, name); 
        }
        public static void RegisterForNavigationWithViewModel(this IContainerRegistry containerRegistry, Type viewType, Type viewModelType)
        {
            containerRegistry.RegisterForNavigationWithViewModel(viewType, viewModelType, viewType.Name);
        }
    }
}
