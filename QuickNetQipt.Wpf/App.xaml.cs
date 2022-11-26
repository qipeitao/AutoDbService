using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace QuickNetQipt.Wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {  
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();

        }
        protected override void InitializeShell(Window shell)
        {
            base.InitializeShell(shell);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {

        } 
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ////弹窗底板
            //containerRegistry.RegisterDialogWindow<DialogHostWindow>();
            //containerRegistry.RegisterDialog<DoubleSetView>();
            //containerRegistry.RegisterDialog<StringSetView>();
            ////关闭
            //containerRegistry.RegisterDialog<CloseWinView>(BlueFluentEngineConst.CloseWinViewName);
            ////containerRegistry.RegisterDialog<ProcessToolTipView>(BlueFluentEngineConst.ProcessToolTipViewName); 

            /////全局错误提示
            //containerRegistry.RegisterForNavigation<ErrorMsg>();
            ////全局等待
            //containerRegistry.RegisterForNavigation<WaitLoad>();
            ////全局登录页面
            //containerRegistry.RegisterForNavigation<LoadView>(BlueFluentEngineConst.LoadViewName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">IsQuietStart StartUserNameByQuiet StartPasswordByQuiet</param>
        protected override void OnStartup(StartupEventArgs e)
        { 
            base.OnStartup(e); 
        }  
        protected override void OnInitialized()
        {
            base.OnInitialized();
        } 
    }
}
