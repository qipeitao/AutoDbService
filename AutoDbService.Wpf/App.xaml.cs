using AutoDbService.DbPrism;
using AutoDbService.DbPrism.Interfaces;
using AutoDbService.Entity;
using AutoDbService.TestAModule;
using AutoDbService.TestBModule;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace AutoDbService.Wpf
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
            moduleCatalog.AddModule<AutoDbService.TestAModule.TestAModule>();
            moduleCatalog.AddModule<AutoDbService.TestBModule.TestBModule>();
             
        } 
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        { 
         }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">IsQuietStart StartUserNameByQuiet StartPasswordByQuiet</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            AutoDbServiceEngine.Instance
                    .UsePrism()
                    .Builder<MyContext>();
            base.OnStartup(e);
           
        }  
        protected override void OnInitialized()
        {
         
            base.OnInitialized();
        }
        
        protected override void ConfigureViewModelLocator()
        {
            base.ConfigureViewModelLocator();
            AutoDbServiceEngine.Instance.ReConfigureViewModelLocator(); 
        }
    }


}
