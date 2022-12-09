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

namespace AutoDbService.DbPrism.Models
{
    public  class ModifyViewModel<TEntity> : AddViewModel<TEntity>,IModifyViewModel<TEntity> where TEntity:EntityBase,new()
    {
        #region baseinfo

        #endregion  
        public override bool IsEdit { get; set; } = true; 
        public ModifyViewModel()
        {
            
        }

        #region 弹窗 
        public override async void OnDialogOpened(IDialogParameters parameters)
        {
            if (!parameters.ContainsKey("entity"))
            {
                //NoticyMsgEvent.Warn("没有加载用户信息!", "警告");
                return;
            }
            var u = parameters.GetValue<TEntity>("entity"); 
            EntityModel = await DbService.FillDetail(s=>s.Id==u.Id);
            NoticyAllProperty();
        }
        #endregion
 
     
    }
}
