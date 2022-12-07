using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using Unity;
using AutoDbService.Entity;
using AutoDbService.Entity.Entities;
using System.Reflection.Emit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Conventions.Infrastructure;
using Microsoft.EntityFrameworkCore.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Internal;
using AutoDbService.Interfaces;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Markup;
using AutoDbService.Extends;
using AutoDbService.Models;
using Prism.Mvvm;
using AutoDbService.DbPrism.Models;
using AutoDbService.DbPrism.Interfaces;
using AutoDbService.DbPrism;

namespace AutoDbService.Wpf
{
    public class MMM: EngineBindableBase
    {
        public virtual string Name { set; get; }
        public virtual void Add()
        {
            Trace.WriteLine($"MMM--Add:");
        }
        //public void Or()
        //{
        //    Trace.WriteLine($"MMM--or:");
        //}
        public virtual void And(int? n)
        {
            Trace.WriteLine($"MMM--And:{n}");
        }
    }
    public class MainWindowViewModel: BindableBase
    { 
        public MMM MM { set; get; }
        public MainWindowViewModel()
        {
            try
            {
                AutoDbServiceEngine.Instance
                    .UsePrism() 
                    .Builder<MyContext>();

                AutoDbServiceEngine.Instance.ReplaceServiceValue<IDbService<User>>(new DbService<User>(s=>s.OrderBy(t=>t.Id),s=>s.Include(t=>t.CreateTeacher)));
                var service= AutoDbServiceEngine.Instance.Get<IDbService<User>>();
                var list=   service.GetListFromDb(out int n);
                list.Wait();
                //using (MyContext db=new MyContext())
                //{
                //    //var teach = new Teacher { Id = Guid.NewGuid() };
                //    //var user = new User { Id = Guid.NewGuid(), CreateTeacherId = teach.Id };
                //    //db.Set<Teacher>().Add(teach);
                //    //db.Set<User>().Add(user);
                //    //db.SaveChanges();
                //    //var uu= db.Set<User>().ToList();
                //    //var tt = db.Set<Teacher>().ToList();
                //    //var pp= db.Set<User>().Include(p => p.CreateTeacher).ToList();
                //    var ss = db.Set<User>().AutoInclude();
                //    var rr = ss.ToList();
                //}
            }
            catch(Exception ex)
            {

            }
            MM = AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType<MMM>(); 
            MM.PropertyChanged += MainWindowViewModel_PropertyChanged;
            MM.Name = "aaaaaa";
            MM.Name = "bbbbb";
             //MM.Add();
            MM.And(10);
            var listt=  MM.GetType().GetRuntimeProperties().FirstOrDefault(p=>p.Name.EndsWith("Command"));
           var command= listt.GetValue(MM) as ICommand;
            command.Execute(null);
        }

        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Trace.WriteLine($"=======PropertyChanged:{e.PropertyName}");
        }

        public virtual string Name { set; get; }
    }
}
