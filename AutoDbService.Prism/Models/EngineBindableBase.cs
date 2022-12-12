using AutoDbService.DbPrism.Attributes;
using AutoDbService.DbPrism.Extends;
using Org.BouncyCastle.Asn1.Ocsp;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Unity;

namespace AutoDbService.DbPrism.Models
{
    /// <summary>
    /// vm基类
    /// </summary>
    public class EngineBindableBase : BindableBase, IDisposable
    {
        [BindingProperty]
        public virtual List<string> Name { set; get; }
        #region 接入 
        public readonly IEventAggregator EventAggregator; 
        public readonly IUnityContainer UnityContainer; 
        public readonly IContainerExtension ContainerExtension;
        /// <summary>
        /// 弹窗服务
        /// </summary> 
        public readonly IDialogService DialogService; 
        #endregion 
    
        public EngineBindableBase()
        {
            try
            {
                UnityContainer = CommonServiceLocator.ServiceLocator.Current.GetInstance<IUnityContainer>();
                EventAggregator = UnityContainer.Resolve<IEventAggregator>(); 
                ContainerExtension = UnityContainer.Resolve<IContainerExtension>(); 
                DialogService = UnityContainer.Resolve<IDialogService>(); 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        /// <summary>
        /// 全部刷新属性
        /// </summary>
        public virtual void NoticyAllProperty()
        {
            this.GetType().GetProperties().ToList().ForEach(p =>
            {
                RaisePropertyChanged(p.Name);
            });
        } 
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        private void SetCommandWhenNull(string fieldName,MethodInfo baseMethod)
        {
            var type = this.GetType();
           var field= type.GetRuntimeFields().FirstOrDefault(p=>p.Name== fieldName);
            if(field!=null&&field.GetValue(this)==null)
            {
                var ps = baseMethod.GetParameters();
                if (ps.Length == 0)
                {
                    var newValue = Activator.CreateInstance(typeof(DelegateCommand), new Action(() => baseMethod.Invoke(this, new object[0])));
                    field.SetValue(this, newValue);
                }
                else
                {
                    if(ps[0].ParameterType.IsValueType)
                    {
                        throw new Exception("参数不能位值类型");
                    }
                    var del = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(ps[0].ParameterType),this,baseMethod);
                    var newValue = Activator.CreateInstance(typeof(DelegateCommand<>).MakeGenericType(ps[0].ParameterType) ,del );
                    field.SetValue(this, newValue);
                }
            }
        }

        public virtual void RequestNavigate(string regName, string viewName, Dictionary<string, string> param = null)
        {
            var RegionManager = UnityContainer.Resolve<IRegionManager>();
            if (!RegionManager.Regions.ContainsRegionWithName(regName))
            {
                Trace.WriteLine($"当前不包含:{regName}");
                return;
            }
            var para = new NavigationParameters();
            if (param != null)
            {
                param.ToList().ForEach(p => para.Add(p.Key, p.Value));
            } 
            var view = RegionManager.Regions[regName].GetView(viewName);
            if (view != null)
            {
                RegionManager.Regions[regName].Activate(view);
            }
            else
            {
                RegionManager.RequestNavigate(regName, viewName, para);
            }
        }
        public static void RaiseProperty(string propertyName)
        {
            //RaisePropertyChanged(propertyName);
        }
    }

    public abstract class DialogEngineBindableBase : EngineBindableBase, IDialogAware, IDataErrorInfo
    {

        #region 弹窗
        /// <summary>
        /// 弹窗标题
        /// </summary>
        [BindingProperty]
        public virtual string Title { set; get; } = "基础弹窗";
        /// <summary>
        /// 关闭事件
        /// </summary>
        public event Action<IDialogResult> RequestClose;

        public virtual bool CanCloseDialog()
        {
            return true;
        }

        public virtual void OnDialogClosed()
        {

        }

        public virtual void OnDialogOpened(IDialogParameters parameters)
        {
            
        }
        #endregion
        #region IDataErrorInfo
        private string _error = string.Empty;
        public string Error
        {
            get { return _error; }
        }
        public virtual string this[string columnName]
        {
            get
            {
                return this.ValidateProperty(columnName);
            }
        }
        #endregion

        #region  弹窗 ok cancel
        #region  Command 
        [Command]
        public abstract void Ok();
        [Command]
        public virtual void Cancel()
        {
            RequestClose_Cancel();
        }
        #endregion

        #endregion
        protected virtual void RequestClose_Ok(IDialogParameters keyValuePairs)
        {
            if (RequestClose == null) return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (RequestClose == null) return;
                RequestClose(new DialogResult(ButtonResult.OK, keyValuePairs));
            }));
        }
        protected virtual void RequestClose_Cancel()
        {
            if (RequestClose == null) return;
            RequestClose(new DialogResult(ButtonResult.Cancel));
        }
    }
}
