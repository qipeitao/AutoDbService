﻿// Copyright (c) 2020 Oracle and/or its affiliates.
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
using System.Text;
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using MySql.Data.MySqlClient;
using MySql.EntityFrameworkCore.Basic.Tests.DbContextClasses;

namespace MySql.EntityFrameworkCore.Basic.Tests.Utils
{
  public class EntityTestsFixtureClass : IDisposable
  {
    // A trace listener to use during testing.
    private AssertFailTraceListener asertFailListener = new AssertFailTraceListener();    
    private readonly IServiceProvider _serviceProvider;
   

    public EntityTestsFixtureClass()      
    {
      //TODO check if we still need this listener
      // Replace existing listeners with listener for testing.
      Trace.Listeners.Clear();
      Trace.Listeners.Add(this.asertFailListener);

      var serviceCollection = new ServiceCollection();

      serviceCollection.AddDbContext<TestsContext>();

      _serviceProvider = serviceCollection.BuildServiceProvider();

    }

    //public void CreateContext(MySQLTestStore testStore)
    //{
    //  var optionsBuilder = new DbContextOptionsBuilder();
    //  optionsBuilder.UseMySQL(MySQLTestStore.CreateConnectionString(_databaseName));

    //  using (var context = new TestsContext(optionsBuilder.Options))
    //  {
    //    context.Database.EnsureDeleted();
    //    context.Database.EnsureCreated();
    //  }
    //}

    public void Dispose()
    {
      // ensure database deletion
      using (var cnn = new MySqlConnection(MySQLTestStore.baseConnectionString))
      {
        cnn.Open();
        var cmd = new MySqlCommand("DROP DATABASE IF EXISTS test", cnn);
        cmd.ExecuteNonQuery();
      }
    }
   

    protected internal void CheckSql(string sql, string refSql)
    {
      StringBuilder str1 = new StringBuilder();
      StringBuilder str2 = new StringBuilder();
      foreach (char c in sql)
        if (!Char.IsWhiteSpace(c))
          str1.Append(c);
      foreach (char c in refSql)
        if (!Char.IsWhiteSpace(c))
          str2.Append(c);
      Assert.AreEqual(0, String.Compare(str1.ToString(), str2.ToString(), true));
    }


    private class AssertFailTraceListener : DefaultTraceListener
    {
      public override void Fail(string message)
      {
        //Assert.Fail("Assertion failure: " + message);
      }

      public override void Fail(string message, string detailMessage)
      {
        //Assert.Fail("Assertion failure: " + detailMessage);
      }
    }
  }

  public class SakilaLiteFixture : IDisposable
  {
    public SakilaLiteFixture()
    {
      using (SakilaLiteContext context = new SakilaLiteContext())
      {
        context.InitContext();
      }
    }

    private void DeleteDatabase<TDbContext>() where TDbContext : MyTestContext, new()
    {
      using(TDbContext context = new TDbContext())
      {
        context.Database.EnsureDeleted();
      }
    }

    #region IDisposable Support
    private bool disposedValue = false; // To detect redundant calls

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue)
      {
        if (disposing)
        {
          DeleteDatabase<SakilaLiteContext>();
          DeleteDatabase<SakilaLiteTableSplittingContext>();
          DeleteDatabase<SakilaLiteUpdateContext>();
        }

        disposedValue = true;
      }
    }

    // This code added to correctly implement the disposable pattern.
    public void Dispose()
    {
      // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
      Dispose(true);
    }
    #endregion

  }
}
