using AutoDbService.DbPrism.Interfaces;
using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Models
{
    public class AutoDbModule : IModule
    {
        public virtual void OnInitialized(IContainerProvider containerProvider)
        {
        }
        public virtual void RegisterTypes(IContainerRegistry containerRegistry)
        {
            AutoDbServiceEngine.Instance.Get<IPrismModuleRegisterService>().DbRegisterTypes(containerRegistry,this); 
        }
    }
}
