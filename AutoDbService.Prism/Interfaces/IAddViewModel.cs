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
    public interface IAddViewModel<TEntity>   where TEntity:EntityBase,new()
    {
        #region baseinfo

        #endregion 
        Guid Id { get; }
        TEntity EntityModel { set; get; }
        bool IsEdit { get; set; }
       IDbService<TEntity> DbService { get; set; }
         
        #region ICommand 
        void Ok();
        void Cancel();
        #endregion 
    }
}
