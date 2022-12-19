using AutoDbService.DbPrism.Attributes;
using AutoDbService.DbPrism.Models;
using AutoDbService.Entity.Entities;
using AutoDbService.Interfaces;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoDbService.TestBModule.ViewModels
{
    public class TeacherViewModel:EngineBindableBase
    {
        public virtual IDbService<Teacher> DbService { get; set; }

        [BindingProperty]
        public virtual ObservableCollection<Teacher> List { set; get; }

        public TeacherViewModel()
        {

        }
         

        [Command]
        public async void Query()
        {
            List = new ObservableCollection<Teacher>();
            List.Clear();
            var ls = await (DbService.GetListFromDb(out int n, p => true));
            List.AddRange(ls);
            //RaisePropertyChanged(nameof(List));
        }
    }
}
