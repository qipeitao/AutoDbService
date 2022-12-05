using AutoDbService.Interfaces;
using AutoDbService.Models;
using AutoDbService.Prism.Extends;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoDbService.Prism.Models
{
    public partial class AddViewModel<TEntity> : EngineBindableBase, IDialogAware, IDataErrorInfo where TEntity:EntityBase,new()
    {
        #region baseinfo

        #endregion 
        public Guid Id { get => EntityModel.Id; }
        public TEntity EntityModel { set; get; } = new TEntity();
        public virtual bool IsEdit { get; set; } = false;
        public virtual IDbService<TEntity> DbService { get; set; }
        public AddViewModel()
        {
            
        }

        #region 弹窗
        /// <summary>
        /// 弹窗标题
        /// </summary>
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
            EntityModel = new TEntity
            {
                Id = Guid.NewGuid(), 
            }; 
            NoticyAllProperty();
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
        #region ICommand 
        public virtual void Ok()
        {
            if (DbService.Add(EntityModel))
            {
                DialogParameters keyValuePairs = new DialogParameters();
                keyValuePairs.Add("ok", EntityModel);
                RequestClose_Ok(keyValuePairs);
            }
            else
            {
                //NoticyMsgEvent.Success("新增失败", Device.Name);
            }

        }
        #endregion

        #region  弹窗 ok cancel
        #region OkCommand
        public DelegateCommand okCommand;
        public DelegateCommand OkCommand
        {
            get
            {
                if (okCommand == null)
                    okCommand = new DelegateCommand(OnOkCommand);
                return okCommand;
            }
        }
        public virtual void OnOkCommand()
        {
            DialogParameters keyValuePairs = new DialogParameters();
            RequestClose_Ok(keyValuePairs);
        }
        public void RequestClose_Ok(IDialogParameters keyValuePairs)
        {
            if (RequestClose == null) return;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                if (RequestClose == null) return;
                RequestClose(new DialogResult(ButtonResult.OK, keyValuePairs));
            }));
        }
        #endregion
        #region CancelCommand
        public DelegateCommand cancelCommand;
        public DelegateCommand CancelCommand
        {
            get
            {
                if (cancelCommand == null)
                    cancelCommand = new DelegateCommand(OnCancelCommand);
                return cancelCommand;
            }
        }
        public virtual void OnCancelCommand()
        {
            if (RequestClose == null) return;
            RequestClose(new DialogResult(ButtonResult.Cancel));
        }
        #endregion  
        #endregion
        
    }
}
