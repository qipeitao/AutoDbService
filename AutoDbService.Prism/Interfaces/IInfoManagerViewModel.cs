using AutoDbService.Interfaces;
using AutoDbService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.DbPrism.Interfaces
{
    public interface IInfoManagerViewModel<TEntity>  where TEntity : EntityBase
    {

        public  string AddEntityDialogName { set; get; }
        public  string ModifyEntityDialogName { set; get; }
        public  IDbService<TEntity> DbService { get; set; }


        #region ICommand
        #region AddCommand

        protected  void AddEntity();

        #endregion
        #region ModifyCommand   
        protected  void ModifyEntity(TEntity entity);
        #endregion

        #region RemoveCommand 
        protected  void RemoveEntity(TEntity s);
        #endregion   
        #endregion
    }
}
