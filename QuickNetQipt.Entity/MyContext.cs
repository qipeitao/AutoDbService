using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using QuickNetQipt.Entity.Entities;
using QuickNetQipt.Engine.Dbs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace QuickNetQipt.Entity
{
  
    public class MyContext : AutoMapContext
    {
        private readonly string connection = ConfigurationManager.AppSettings["DBConnection"] ?? "Server=localhost;port=3306;userid=root;password=admin;persist security info=false;charset=utf8mb4;database=bluefluent;sslMode=none;AllowPublicKeyRetrieval=true;";
        public MyContext() : base()
        {
            //Trace.WriteLine("++++++++++++++++++++++++++++++++++++++++++");
        } 

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(connection);
        } 
    }
}