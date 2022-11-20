// Copyright (c) 2020 Oracle and/or its affiliates.
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License, version 2.0, as
// published by the Free Software Foundation.
//
// This program is also distributed with certain software (including
// but not limited to OpenSSL) that is licensed under separate terms,
// as designated in a particular file or component or in included license
// documentation.  The authors of MySQL hereby grant you an
// additional permission to link the program and your derivative works
// with the separately licensed software that they have included with
// MySQL.
//
// Without limiting anything contained in the foregoing, this file,
// which is part of MySQL Connector/NET, is also subject to the
// Universal FOSS Exception, version 1.0, a copy of which can be found at
// http://oss.oracle.com/licenses/universal-foss-exception.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
// See the GNU General Public License, version 2.0, for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using System;
using System.Linq;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore.Diagnostics;
using MySql.EntityFrameworkCore.Basic.Tests.DbContextClasses;
using MySql.EntityFrameworkCore.Basic.Tests.Utils;

namespace MySql.EntityFrameworkCore.Basic.Tests
{
  public class FunctionalTests
  {
    [OneTimeTearDown]
    public void OneTimeTearDown()
    {
      using (WorldContext context = new WorldContext())
        context.Database.EnsureDeleted();
    }

    [Test]
    public void CanConnectWithConnectionOnConfiguring()
    {
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddEntityFrameworkMySQL()
        .AddDbContext<ConnStringOnConfiguringContext>();

     var serviceProvider = serviceCollection.BuildServiceProvider();  

      using (var context = serviceProvider.GetRequiredService<ConnStringOnConfiguringContext>())
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        Assert.True(context.Posts.Count() == 0);
        context.Database.EnsureDeleted();
      }
    }


    [Test]
    public void CanThrowExceptionWhenNoConfiguration()
    {
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddEntityFrameworkMySQL()
        .AddDbContext<NoConfigurationContext>();

      var serviceProvider = serviceCollection.BuildServiceProvider();

      using (var context = serviceProvider.GetRequiredService<NoConfigurationContext>())
      {
        Assert.AreEqual(CoreStrings.NoProviderConfigured, Assert.Throws<InvalidOperationException>(() => context.Blogs.Any()).Message);        
      }
    }


    [Test]
    public void CreatedDb()
    {
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddEntityFrameworkMySQL()
        .AddDbContext<TestsContext>();

      var serviceProvider = serviceCollection.BuildServiceProvider();

      using (var context = serviceProvider.GetRequiredService<TestsContext>())
      {
        context.Database.EnsureCreated();
        var dbname = context.Database.GetDbConnection().Database;
        using (var cnn = new MySqlConnection(MySQLTestStore.baseConnectionString + string.Format(";database={0}", context.Database.GetDbConnection().Database)))
        {
          cnn.Open();
          var cmd = new MySqlCommand(string.Format("SHOW DATABASES LIKE '{0}'", context.Database.GetDbConnection().Database), cnn);
          var reader = cmd.ExecuteReader();
          while (reader.Read())
          {
            Assert.True(string.Equals(dbname, reader.GetString(0), StringComparison.CurrentCultureIgnoreCase), "Database was not created");
          }
        }
        context.Database.EnsureDeleted();
      }
    }


    [Test]
    public void EnsureRelationalPatterns()
    {
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddEntityFrameworkMySQL()
        .AddDbContext<TestsContext>();

      var serviceProvider = serviceCollection.BuildServiceProvider();

      using (var context = serviceProvider.GetRequiredService<TestsContext>())
      {
        context.Database.EnsureCreated();
        using (var cnn = new MySqlConnection(MySQLTestStore.baseConnectionString + string.Format(";database={0}", context.Database.GetDbConnection().Database)))
        {
          var dbname = context.Database.GetDbConnection().Database;
          cnn.Open();
          var cmd = new MySqlCommand(string.Format("SHOW DATABASES LIKE '{0}'", context.Database.GetDbConnection().Database), cnn);
          var reader = cmd.ExecuteReader();
          while (reader.Read())
          {
            Assert.True(string.Equals(dbname, reader.GetString(0), StringComparison.CurrentCultureIgnoreCase), "Database was not created");
          }
        }
        context.Database.EnsureDeleted();
      }
    }


    [Test]
    public void CanUseIgnoreEntity()
    {
      var serviceCollection = new ServiceCollection();
      serviceCollection.AddEntityFrameworkMySQL()
        .AddDbContext<SimpleContextWithIgnore>();

      var serviceProvider = serviceCollection.BuildServiceProvider();

      using (var context = serviceProvider.GetRequiredService<SimpleContextWithIgnore>())
      {
        context.Database.EnsureCreated();
        Assert.True(context.Model.GetEntityTypes().Count() == 2, "Wrong model generation");
        Assert.True(context.Blogs.ToList().Count == 0, "");        
        context.Database.EnsureDeleted();
      }
    }



    [Test]
    public void CanUseOptionsInDbContextCtor()
    {                      
      using (var context = new OptionsContext(new DbContextOptions<OptionsContext>(),
                                              new MySqlConnection(MySQLTestStore.CreateConnectionString("db-optionsindbcontext"))))
      {
        context.Database.EnsureCreated();     
        Assert.True(context.Blogs.Count() == 0);
        context.Database.EnsureDeleted();
      }

    }

    [Test]
    public void TestEnsureSchemaOperation()
    {
      using(var context = new WorldContext())
      {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
        context.Countries.Add(new Country()
        {
          Code = "1",
          Name = "London"
        });
        context.SaveChanges();
      }
    }

    #region ContextClasses

    protected class OptionsContext : SimpleContext
    {
      private readonly MySqlConnection _connection;
      private readonly DbContextOptions _options;

      public OptionsContext(DbContextOptions options, MySqlConnection connection)
          : base(options)
      {
        _options = options;
        _connection = connection;
      }

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
        Assert.AreSame(_options, optionsBuilder.Options);

        optionsBuilder.UseMySQL(_connection);

        Assert.AreNotSame(_options, optionsBuilder.Options);
      }

      public override void Dispose()
      {
        _connection.Dispose();
        base.Dispose();
      }
    }

    private class ConnStringOnConfiguringContext : TestsContext
    {
      public ConnStringOnConfiguringContext(DbContextOptions options) 
        : base(options)
      {
      }      
    }


    private class UseConnectionInOnConfiguring : TestsContext
    {
      private MySqlConnection _connection;

      public UseConnectionInOnConfiguring(MySqlConnection connection)
      {
        _connection = connection;
      }      

      protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
      {
        optionsBuilder.UseMySQL(_connection);
      }

      public override void Dispose()
      {
        _connection.Dispose();
        base.Dispose();
      }
    }
    #endregion
  }
}
