using AutoDbService.Interfaces;
using AutoDbService.Models;
using AutoDbService.DbPrism.Interfaces;
using Org.BouncyCastle.Asn1.Ocsp;
using Prism.Events;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using System.Collections.ObjectModel;
using AutoDbService.DbPrism.Attributes;
using System.Windows.Documents;

namespace AutoDbService.DbPrism.Models
{
    /// <summary>
    /// 信息管理类基类
    /// 实现一个dbservice
    /// 实现通用 curd
    /// </summary>
    public class InfoManagerViewModel<TEntity> : EngineBindableBase, IInfoManagerViewModel<TEntity> where TEntity : EntityBase
    {
 
        public virtual string AddEntityDialogName { set; get; }
        public virtual string ModifyEntityDialogName { set; get; }
        public virtual IDbService<TEntity> DbService { get; set; }
         
        public virtual List<string> List { set; get; }

        [BindingProperty]
        public virtual string Items { set; get; }
        public InfoManagerViewModel()
        {
             
        }

        #region ICommand
        #region AddCommand

        public virtual void AddEntity()
        {
            DialogService.ShowDialog(AddEntityDialogName, new DialogParameters(), r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var dev = r.Parameters.GetValue<TEntity>("ok");
                    if (dev != null)
                    { 
                        DbService?.Add(dev);
                    }
                }
            });
        }

        #endregion
        #region ModifyCommand   
        public virtual void ModifyEntity(TEntity entity)
        {
            DialogParameters keyValuePairs = new DialogParameters
            {
                {"entity",entity},
            };
            DialogService.ShowDialog(ModifyEntityDialogName, keyValuePairs, r =>
            {
                if (r.Result == ButtonResult.OK)
                {
                    var dev = r.Parameters.GetValue<TEntity>("ok");
                    if (dev != null)
                    {
                        DbService.Save(entity);
                    }
                }
            });
        }
        #endregion

        #region RemoveCommand 
        public virtual void RemoveEntity(TEntity s)
        {
            DbService?.Remove(s);
        }
        #endregion   
        #endregion

        [Command]
        public async void Query()
        {
            List = new List<string>();
            Items = "";
            //List = List ?? new List<TEntity>(); 
            //var ls = (await DbService.GetListFromDb(out int n, p => true));
            //List.AddRange(ls);
        }
    }
}
