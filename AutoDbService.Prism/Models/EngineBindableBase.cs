using Org.BouncyCastle.Asn1.Ocsp;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Unity;

namespace AutoDbService.DbPrism.Models
{
    /// <summary>
    /// vm基类
    /// </summary>
    public class EngineBindableBase : BindableBase, IDisposable
    {
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
                EventAggregator = UnityContainer.Resolve<IEventAggregator>();
                UnityContainer = UnityContainer.Resolve<IUnityContainer>();
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
                    var newValue = Activator.CreateInstance(typeof(DelegateCommand), new Action(() => baseMethod.Invoke(this, null)));
                    field.SetValue(this, newValue);
                }
                else
                {
                    var del = Delegate.CreateDelegate(typeof(Action<>).MakeGenericType(ps[0].ParameterType),this,baseMethod);
                    var newValue = Activator.CreateInstance(typeof(DelegateCommand<>).MakeGenericType(ps[0].ParameterType), del);
                    field.SetValue(this, newValue);
                }
            }
        } 
    }
}
