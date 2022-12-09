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
    /// <summary>
    /// 默认继承IAddViewModel
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IModifyViewModel<TEntity> : IAddViewModel<TEntity> where TEntity:EntityBase,new()
    { 

    }
}
