using Microsoft.Xaml.Behaviors.Layout;
using Prism.Ioc;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows;
using Unity;
using QuickNetQipt.Entity;
using QuickNetQipt.Entity.Entities;
using Microsoft.EntityFrameworkCore;
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

namespace QuickNetQipt.Wpf
{ 
    public class MainWindowViewModel
    { 
        public MainWindowViewModel() : base()
        {
            try
            { 
                using (MyContext db = new())
                {
                    //    db.User.Add(new Entity.Entities.User
                    //    {
                    //        Id = Guid.NewGuid(),
                    //    });
                    db.Set<User>().Add(new Entity.Entities.User
                    {
                        Id = Guid.NewGuid(),
                    });
                    db.SaveChanges();
                }
            }catch(Exception ex)
            {

            }
        } 
    }
}
