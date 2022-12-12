using Prism.Ioc;
using Prism.Modularity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace AutoDbService.DbPrism.Interfaces
{
    /// <summary>
    /// 将模块内注册内容 提取并自动关联
    /// </summary>
    public interface IPrismModuleRegisterService
    {
        void DbRegisterTypes(IContainerRegistry containerRegistry, object obj);
        void DbRegisterTypes(IUnityContainer unityRegistry, object obj);

        void DbRegisterType<View,ViewModel>(IUnityContainer unityContainer);
    }
}
