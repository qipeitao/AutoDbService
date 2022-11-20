﻿// Copyright (c) 2020, 2022, Oracle and/or its affiliates.
//
// MySQL Connector/NET is licensed under the terms of the GPLv2
// <http://www.gnu.org/licenses/old-licenses/gpl-2.0.html>, like most 
// MySQL Connectors. There are special exceptions to the terms and 
// conditions of the GPLv2 as it is applied to this software, see the 
// FLOSS License Exception
// <http://www.mysql.com/about/legal/licensing/foss-exception.html>.
//
// This program is free software; you can redistribute it and/or modify 
// it under the terms of the GNU General Public License as published 
// by the Free Software Foundation; version 2 of the License.
//
// This program is distributed in the hope that it will be useful, but 
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY 
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License 
// for more details.
//
// You should have received a copy of the GNU General Public License along 
// with this program; if not, write to the Free Software Foundation, Inc., 
// 51 Franklin St, Fifth Floor, Boston, MA 02110-1301  USA

using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Transactions;
using NUnit.Framework;
using MySql.EntityFrameworkCore.Basic.Tests.Utils;
using MySql.EntityFrameworkCore.Basic.Tests.DbContextClasses;

namespace MySql.EntityFrameworkCore.Basic.Tests
{
  public class EFCoreTests : SakilaLiteFixture
  {
    private SakilaLiteFixture fixture;

    [SetUp]
    public void Init()
    {
      this.fixture = new SakilaLiteFixture();
    }

    // Explicitly compiled query
    private static Func<SakilaLiteContext, int, Customer> _customerById =
      EF.CompileQuery((SakilaLiteContext context, int id) =>
        context.Customer.Single(p => p.CustomerId == id));

    [Test]
    public void ExplicitlyCompiledQueries()
    {
      using (SakilaLiteContext context = new SakilaLiteContext())
      {
        var customer = _customerById(context, 9);
        Assert.AreEqual(9, customer.CustomerId);
        Assert.AreEqual("MOORE", customer.LastName);
      }
    }

    [Test]
    public void GraphNewAndExistingEntities()
    {
      using (SakilaLiteUpdateContext context = new SakilaLiteUpdateContext())
      {
        context.InitContext();

        Actor actorNoId, actor;
        context.Attach(actorNoId = new Actor { FirstName = "PENELOPE", LastName = "GUINESS" });
        context.Attach(actor = new Actor { ActorId = 21, FirstName = "KIRSTEN", LastName = "PALTROW" });

        var changes = context.ChangeTracker.Entries();
        Assert.AreEqual(2, changes.Count());
        Assert.That(changes.Select(c => c.State), Has.Exactly(1).Matches<EntityState>(state => state.Equals(EntityState.Added)));
        Assert.That(changes.Select(c => c.State), Has.Exactly(1).Matches<EntityState>(state => state.Equals(EntityState.Unchanged)));

        context.SaveChanges();

        var list = context.Actor.ToList();
        Assert.AreEqual(1, list.Count);
      }
    }

    [Test]
    public void StringInterpolationInSqlCommands()
    {
      using (SakilaLiteUpdateContext context = new SakilaLiteUpdateContext())
      {
        context.InitContext();

        int id = 1;
        string firstName = "PENELOPE";
        string lastName = "GUINESS";
        DateTime lastUpdate = DateTime.Parse("2006-02-15 04:34:33");
        context.Database.ExecuteSqlInterpolated($"INSERT INTO actor(actor_id, first_name, last_name, last_update) VALUES ({id}, {firstName}, {lastName}, {lastUpdate:u})");
        Actor actor = context.Set<Actor>().FromSqlInterpolated($"SELECT * FROM actor WHERE actor_id={id} and last_name={lastName}").First();

        Assert.AreEqual(id, actor.ActorId);
        Assert.AreEqual(firstName, actor.FirstName);
      }
    }

    [Test]
    public void TransactionScopeTest()
    {
      using (var context = new SakilaLiteUpdateContext())
      {
        context.InitContext(false);
      }

      using (TransactionScope scope = new TransactionScope(TransactionScopeOption.Required,
        new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }))
      {
        using (MySqlConnection connection = new MySqlConnection(MySQLTestStore.GetContextConnectionString<SakilaLiteUpdateContext>()))
        {
          connection.Open();

          MySqlCommand command = connection.CreateCommand();
          command.CommandText = "DELETE FROM actor";
          command.ExecuteNonQuery();

          var options = new DbContextOptionsBuilder<SakilaLiteUpdateContext>()
            .UseMySQL(connection)
            .Options;

          using (TransactionScope innerScope = new TransactionScope(TransactionScopeOption.Required))
          {
            using (var context = new SakilaLiteUpdateContext(options))
            {
              context.Actor.Add(new Actor
              {
                FirstName = "PENELOPE",
                LastName = "GUINESS"
              });
              context.SaveChanges();
              innerScope.Complete();
            }
          }

          // Commit transaction if all commands succeed, transaction will auto-rollback
          // when disposed if either commands fails
          scope.Complete();
        }
      }
    }

    /// <summary>
    /// Bug #103436 SqlNullabilityProcessor error when using EF and filter by Date/Time
    /// </summary>
    [Test]
    public void QueryWithDate()
    {
      using (SakilaLiteContext context = new SakilaLiteContext())
      {
        var scenario1 = context.Actor.Where(
        w => w.FirstName == "GARY"
        && w.LastName == "PENN"
        && w.LastUpdate >= DateTime.Today.AddDays(-15));

        var res = scenario1.Count();
        Assert.AreEqual(0, res);

        var dt = DateTime.Today.AddDays(-15);
        var scenario2 = context.Actor.Where(
        w => w.FirstName == "GARY"
        && w.LastName == "PENN"
        && w.LastUpdate >= dt);

        var res2 = scenario2.Count();
        Assert.AreEqual(0, res2);

        var scenario3 = context.Actor.Where(
        w => w.FirstName == "GARY"
        && w.LastName == "PENN"
        && w.LastUpdate >= DateTime.Today.AddDays(3));

        var res3 = scenario3.Count();
        Assert.AreEqual(0, res3);
      }
    }

  }
}
