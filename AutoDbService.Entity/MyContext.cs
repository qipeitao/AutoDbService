using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using AutoDbService.Entity.Entities;
using AutoDbService.Dbs;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Emit;

namespace AutoDbService.Entity
{
  
    public class MyContext : AutoMapContext
    {
        private readonly string connection = ConfigurationManager.AppSettings["DBConnection"] ?? "Server=localhost;port=3306;userid=root;password=admin;persist security info=false;charset=utf8mb4;database=quick;sslMode=none;AllowPublicKeyRetrieval=true;";
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