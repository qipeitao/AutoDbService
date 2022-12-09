using AutoDbService.Interfaces;
using AutoDbService.Models;
using AutoDbService.DbPrism.Extends;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using AutoDbService.DbPrism.Attributes;

namespace AutoDbService.DbPrism.Models
{
    public  class AddViewModel<TEntity> : DialogEngineBindableBase, IDialogAware, IDataErrorInfo where TEntity:EntityBase,new()
    {
        #region baseinfo

        #endregion 
        public Guid Id { get => EntityModel.Id; }
        public TEntity EntityModel { set; get; } = new TEntity();
        public virtual bool IsEdit { get; set; } = false;
        
        [DbType]
        public virtual IDbService<TEntity> DbService { get; set; } 

        #region 弹窗 
        public override void OnDialogOpened(IDialogParameters parameters)
        {
            EntityModel = new TEntity
            {
                Id = Guid.NewGuid(), 
            }; 
            NoticyAllProperty();
        }
        #endregion 
         
        #region  弹窗 ok cancel
        #region  Command 
        public override void Ok()
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

        #endregion 
    }
}
