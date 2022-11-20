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
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text;
using Microsoft.EntityFrameworkCore.Storage;
using MySql.EntityFrameworkCore.Utils;

namespace MySql.EntityFrameworkCore.Migrations.Internal
{
  /// <summary>
  ///     <para>
  ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
  ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
  ///         any release. You should only use it directly in your code with extreme caution and knowing that
  ///         doing so can result in application failures when updating to a new Entity Framework Core release.
  ///     </para>
  ///     <para>
  ///         The service lifetime is <see cref="ServiceLifetime.Scoped" />. This means that each
  ///         <see cref="DbContext" /> instance will use its own instance of this service.
  ///         The implementation may depend on other services registered with any lifetime.
  ///         The implementation does not need to be thread-safe.
  ///     </para>
  /// </summary>
  internal partial class MySQLHistoryRepository : HistoryRepository
  {
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public MySQLHistoryRepository([NotNull] HistoryRepositoryDependencies dependencies)
  : base(dependencies)
    {
    }

    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    protected override string ExistsSql
    {
      get
      {
        var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));
        var builder = new StringBuilder();

        builder.Append("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE ");

        builder
            .Append("TABLE_SCHEMA=")
            .Append(stringTypeMapping.GenerateSqlLiteral(TableSchema ?? Dependencies.Connection.DbConnection.Database))
            .Append(" AND TABLE_NAME=")
            .Append(stringTypeMapping.GenerateSqlLiteral(TableName))
            .Append(";");

        return builder.ToString();
      }
    }


    protected override bool InterpretExistsResult(object value) => value != null;

    public override string GetBeginIfExistsScript(string migrationId)
    {
      Check.NotEmpty(migrationId, nameof(migrationId));

      var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

      return new StringBuilder()
          .Append("IF EXISTS(SELECT * FROM ")
          .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
          .Append(" WHERE ")
          .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
          .Append(" = ")
          .Append(stringTypeMapping.GenerateSqlLiteral(migrationId))
          .AppendLine(")")
          .Append("BEGIN")
          .ToString();
    }

    public override string GetBeginIfNotExistsScript(string migrationId)
    {
      Check.NotEmpty(migrationId, nameof(migrationId));

      var stringTypeMapping = Dependencies.TypeMappingSource.GetMapping(typeof(string));

      return new StringBuilder()
          .Append("IF NOT EXISTS(SELECT * FROM ")
          .Append(SqlGenerationHelper.DelimitIdentifier(TableName, TableSchema))
          .Append(" WHERE ")
          .Append(SqlGenerationHelper.DelimitIdentifier(MigrationIdColumnName))
          .Append(" = ")
          .Append(stringTypeMapping.GenerateSqlLiteral(migrationId))
          .AppendLine(")")
          .Append("BEGIN")
          .ToString();
    }

    public override string GetCreateIfNotExistsScript()
    {
      var script = GetCreateScript();
      return script.Insert(script.IndexOf("CREATE TABLE", StringComparison.Ordinal) + 12, " IF NOT EXISTS");
    }

    public override string GetEndIfScript() => "END;" + Environment.NewLine;

  }
}
