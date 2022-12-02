﻿using Microsoft.Xaml.Behaviors.Layout;
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
using QuickNetQipt.Entity;
using QuickNetQipt.Entity.Entities; 
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
using QuickNetQipt.Engine;
using QuickNetQipt.Engine.Interfaces;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf.WellKnownTypes;
using System.Windows.Markup;
using QuickNetQipt.Engine.Extends;
using QuickNetQipt.Engine.Models;

namespace QuickNetQipt.Wpf
{ 
    public class MainWindowViewModel
    { 
        public MainWindowViewModel() : base()
        {
            try
            {
                QuickNetQiptEngine.Instance.Builder<MyContext>();

                QuickNetQiptEngine.Instance.ReplaceServiceValue<IDbService<User>>(new DbService<User>(s=>s.OrderBy(t=>t.Id),s=>s.Include(t=>t.CreateTeacher)));
                var service= QuickNetQiptEngine.Instance.Get<IDbService<User>>();
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
        } 
    }
}