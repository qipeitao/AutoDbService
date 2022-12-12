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
using AutoDbService.DbPrism.Attributes;

namespace AutoDbService.Wpf
{
 
    public class MainWindowViewModel: EngineBindableBase
    {
        [Command]
       public void ToA()
        {
            RequestNavigate("MainRegion", "UserView");
        }
        [Command]
        public void ToB()
        {
            RequestNavigate("MainRegion", "TeacherView");
        }
        [Command]
        public void ToC(string a)
        {
            Trace.WriteLine("ToC:"+a);
        }
        public MainWindowViewModel()
        {
            try
            {
                var ss=AutoDbServiceEngine.Instance.Get<IBuildDynamicType>().BuildType<EngineBindableBase>();
                ss.PropertyChanged += MainWindowViewModel_PropertyChanged;
                ss.Name = new List<string>() { "1","2"};
                Trace.WriteLine(ss.Name);
                //AutoDbServiceEngine.Instance.ReplaceServiceValue<IDbService<User>>(new DbService<User>(s=>s.OrderBy(t=>t.Id),s=>s.Include(t=>t.CreateTeacher)));
                //var service= AutoDbServiceEngine.Instance.Get<IDbService<User>>();
                //var list=   service.GetListFromDb(out int n);
                //list.Wait();
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
        }

 
        private void MainWindowViewModel_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Trace.WriteLine($"=======PropertyChanged:{e.PropertyName}");
        }

       
    }
}
