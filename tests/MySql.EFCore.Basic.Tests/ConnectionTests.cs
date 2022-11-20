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

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using Microsoft.Extensions.Logging;
using MySql.EntityFrameworkCore.Diagnostics.Internal;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using NUnit.Framework;
using MySql.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Basic.Tests.DbContextClasses;
using MySql.EntityFrameworkCore.Basic.Tests.Utils;
using System;

namespace MySql.EntityFrameworkCore.Basic.Tests
{
  public partial class ConnectionTests
  {
    private static MySQLServerConnection CreateConnection(DbContextOptions options)
    {
      var dependencies = CreateDependencies(options);

      return new MySQLServerConnection(dependencies);
    }

    public static RelationalConnectionDependencies CreateDependencies(DbContextOptions options = null)
    {
      options ??= new DbContextOptionsBuilder()
          .UseMySQL(MySQLTestStore.baseConnectionString + "database=test;")
          .Options;

      return new RelationalConnectionDependencies(
          options,
          new DiagnosticsLogger<DbLoggerCategory.Database.Transaction>(
              new LoggerFactory(),
              new LoggingOptions(),
              new DiagnosticListener("FakeDiagnosticListener"),
              new MySQLLoggingDefinitions(),null),
          new DiagnosticsLogger<DbLoggerCategory.Database.Connection>(
              new LoggerFactory(),
              new LoggingOptions(),
              new DiagnosticListener("FakeDiagnosticListener"),
              new MySQLLoggingDefinitions(),null),
          new NamedConnectionStringResolver(options),
          new RelationalTransactionFactory(new RelationalTransactionFactoryDependencies()),
          new CurrentDbContext(new FakeDbContext()));
    }

    [TearDown]
    public void TearDown()
    {
      using (var context = new SakilaLiteUpdateContext())
      {
        context.DropContext();
      }
    }

    private class FakeDbContext : DbContext
    {
    }

    [Test]
    public void CanCreateConnectionString()
    {
      using (var connection = CreateConnection(CreateOptions()))
      {
        Assert.IsInstanceOf<MySqlConnection>(connection.DbConnection);
      }
    }

    [Test]
    public void CanCreateMainConnection()
    {
      using (var connection = CreateConnection(CreateOptions()))
      {
        using (var source = connection.CreateSourceConnection())
        {
          var csb = new MySqlConnectionStringBuilder(source.ConnectionString);
          var csb1 = new MySqlConnectionStringBuilder(MySQLTestStore.baseConnectionString + "database=mysql");
          Assert.True(csb.Database == csb1.Database);
          Assert.True(csb.Port == csb1.Port);
          Assert.True(csb.Server == csb1.Server);
          Assert.True(csb.UserID == csb1.UserID);
        }
      }
    }

    public static DbContextOptions CreateOptions()
    {
      var optionsBuilder = new DbContextOptionsBuilder();
      optionsBuilder.UseMySQL(MySQLTestStore.baseConnectionString + "database=test;");
      return optionsBuilder.Options;
    }

    [Test]
    public void TransactionTest()
    {
      using (var context = new SakilaLiteUpdateContext())
      {
        context.InitContext(false);
        MySqlTrace.LogInformation(9966, "EF Model CREATED");
      }

      using (MySqlConnection connection = new MySqlConnection(MySQLTestStore.GetContextConnectionString<SakilaLiteUpdateContext>()))
      {
        connection.Open();

        using (MySqlTransaction transaction = connection.BeginTransaction())
        {

          MySqlCommand command = connection.CreateCommand();
          command.CommandText = "DELETE FROM actor";
          command.ExecuteNonQuery();

          var options = new DbContextOptionsBuilder<SakilaLiteUpdateContext>()
            .UseMySQL(connection)
            .Options;

          using (var context = new SakilaLiteUpdateContext(options))
          {
            context.Database.UseTransaction(transaction);
            context.Actor.Add(new Actor
            {
              FirstName = "PENELOPE",
              LastName = "GUINESS"
            });
            context.SaveChanges();
          }

          transaction.Commit();
        }
      }
    }
  }
}
