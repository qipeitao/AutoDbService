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

using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq;
using MySql.EntityFrameworkCore.Migrations.Operations;
using MySql.EntityFrameworkCore.Metadata.Internal;
using MySql.EntityFrameworkCore.Utils;
using System.Text.RegularExpressions;
using MySql.EntityFrameworkCore.Internal;
using MySql.EntityFrameworkCore.Metadata;
using MySql.EntityFrameworkCore.Infrastructure.Internal;
using MySql.EntityFrameworkCore.Infrastructure;
using System.Reflection;
using System;
using Microsoft.EntityFrameworkCore;

namespace MySql.EntityFrameworkCore.Migrations
{
  /// <summary>
  /// MigrationSqlGenerator implementation for MySQL
  /// </summary>
  internal class MySQLMigrationsSqlGenerator : MigrationsSqlGenerator
  {
    private static readonly Regex _typeRegex = new Regex(@"([a-z0-9]+)\s*?(?:\(\s*(\d+)?\s*\))?",
    RegexOptions.IgnoreCase);

    private RelationalTypeMapping _typeMapper;
    private readonly IRelationalAnnotationProvider _annotationProvider;
    private readonly IMySQLOptions _options;

    public MySQLMigrationsSqlGenerator(
      [NotNull] MigrationsSqlGeneratorDependencies dependencies, 
      [NotNull] IRelationalAnnotationProvider annotationProvider,
      [NotNull] IMySQLOptions options)
        : base(dependencies)
    {
      _annotationProvider = annotationProvider;
      _options = options;
      _typeMapper = dependencies.TypeMappingSource.GetMapping(typeof(string));
    }

    protected override void Generate(
      [NotNull] MigrationOperation operation,
      [CanBeNull] IModel model,
      [NotNull] MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      if (operation is MySQLCreateDatabaseOperation)
        Generate(operation as MySQLCreateDatabaseOperation, model, builder);
      else if (operation is MySQLDropDatabaseOperation)
        Generate(operation as MySQLDropDatabaseOperation, model, builder);
      else
        base.Generate(operation, model, builder);
    }

    protected override void Generate(
      [NotNull] EnsureSchemaOperation operation,
      [CanBeNull] IModel model,
      [NotNull] MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
        .Append("CREATE DATABASE IF NOT EXISTS ")
        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

      EndStatement(builder, suppressTransaction: true);
    }

    protected virtual void Generate(
        [NotNull] MySQLCreateDatabaseOperation operation,
        [CanBeNull] IModel model,
        MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
          .Append("CREATE DATABASE ")
          .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name));

      EndStatement(builder, suppressTransaction: true);
    }

    protected virtual void Generate(
        [NotNull] MySQLDropDatabaseOperation operation,
        [CanBeNull] IModel model,
        [NotNull] MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
          .Append("DROP DATABASE IF EXISTS ")
          .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
          .Append(Dependencies.SqlGenerationHelper.StatementTerminator)
          .AppendLine(Dependencies.SqlGenerationHelper.BatchTerminator);

      EndStatement(builder);
    }

    /// <summary>
    ///     Generates a SQL fragment for a column definition in an <see cref="AddColumnOperation" />.
    /// </summary>
    /// <param name="operation"> The operation. </param>
    /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
    /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
    protected override void ColumnDefinition(AddColumnOperation operation, IModel model,
        MigrationCommandListBuilder builder)
        => ColumnDefinition(
            operation.Schema,
            operation.Table,
            operation.Name,
            operation,
            model,
            builder);

    /// <summary>
    ///     Generates a SQL fragment for a column definition for the given column metadata.
    /// </summary>
    /// <param name="schema"> The schema that contains the table, or <c>null</c> to use the default schema. </param>
    /// <param name="table"> The table that contains the column. </param>
    /// <param name="name"> The column name. </param>
    /// <param name="operation"> The column metadata. </param>
    /// <param name="model"> The target model which may be <c>null</c> if the operations exist without a model. </param>
    /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
    protected override void ColumnDefinition(
       string schema,
       string table,
       string name,
       ColumnOperation operation,
       IModel model,
       MigrationCommandListBuilder builder)
    {
      Check.NotEmpty(name, nameof(name));
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      var matchType = operation.ColumnType ?? GetColumnType(schema, table, name, operation, model);
      var matchLen = "";
      var match = _typeRegex.Match(matchType ?? "-");
      if (match.Success)
      {
        matchType = match.Groups[1].Value.ToLower();
        if (!string.IsNullOrWhiteSpace(match.Groups[2].Value))
        {
          matchLen = match.Groups[2].Value;
        }
      }

      var valueGenerationStrategy = MySQLValueGenerationStrategyCompatibility.GetValueGenerationStrategy(operation.GetAnnotations().OfType<IAnnotation>().ToArray());

      var autoIncrement = false;
      if (valueGenerationStrategy == MySQLValueGenerationStrategy.IdentityColumn &&
          string.IsNullOrWhiteSpace(operation.DefaultValueSql) && operation.DefaultValue == null)
      {
        switch (matchType)
        {
          case "tinyint":
          case "smallint":
          case "mediumint":
          case "int":
          case "bigint":
            autoIncrement = true;
            break;
          case "datetime":
          case "timestamp":
            operation.DefaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
            break;
        }
      }

      string onUpdateSql = null;
      if (operation.IsRowVersion || valueGenerationStrategy == MySQLValueGenerationStrategy.ComputedColumn)
      {
        switch (matchType)
        {
          case "datetime":
          case "timestamp":
            if (string.IsNullOrWhiteSpace(operation.DefaultValueSql) && operation.DefaultValue == null)
            {
              operation.DefaultValueSql = $"CURRENT_TIMESTAMP({matchLen})";
            }

            onUpdateSql = $"CURRENT_TIMESTAMP({matchLen})";
            break;
        }
      }

      if (operation.ComputedColumnSql == null)
      {
        ColumnDefinitionWithCharSet(schema, table, name, operation, model, builder);

        if (autoIncrement)
        {
          builder.Append(" AUTO_INCREMENT");
        }
        else
        {
          if (onUpdateSql != null)
          {
            builder
                .Append(" ON UPDATE ")
                .Append(onUpdateSql);
          }
        }
      }
      else
      {
        builder
            .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
            .Append(" ")
            .Append(operation.ColumnType ?? GetColumnType(schema, table, name, operation, model));
        builder
            .Append(" AS ")
            .Append($"({operation.ComputedColumnSql})");

        if (operation.IsNullable)
        {
          builder.Append(" NULL");
        }
      }
    }

    private void ColumnDefinitionWithCharSet(string schema, string table, string name, ColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
    {
      if (operation.ComputedColumnSql != null)
      {
        ComputedColumnDefinition(schema, table, name, operation, model, builder);
        return;
      }

      var property = (model.GetRelationalModel().FindTable(table, schema)?.FindColumn(name) != null)
            ? FindProperty(model, schema, table, name) : null;

      var columnType = operation.ColumnType != null
        ? GetColumnTypeWithCharSetAndCollation(operation, operation.ColumnType, property)
        : GetColumnType(schema, table, name, operation, model);

      builder
          .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(name))
          .Append(" ")
          .Append(columnType);

      builder.Append(operation.IsNullable ? " NULL" : " NOT NULL");

      DefaultValue(operation.DefaultValue, operation.DefaultValueSql, columnType, builder);
    }

    private string GetColumnTypeWithCharSetAndCollation(ColumnOperation operation, string columnType, IProperty property)
    {
      var charSet = property != null ? property[MySQLAnnotationNames.Charset] : operation[MySQLAnnotationNames.Charset];
      if (charSet != null)
      {
        const string characterSetClausePattern = @"CHARACTER SET \w+";
        var characterSetClause = $@"CHARACTER SET {charSet}";

        columnType = Regex.IsMatch(columnType, characterSetClausePattern, RegexOptions.IgnoreCase)
            ? Regex.Replace(columnType, characterSetClausePattern, characterSetClause)
            : columnType.TrimEnd() + " " + characterSetClause;
      }

      var collation = property != null ? property[MySQLAnnotationNames.Collation] : operation[MySQLAnnotationNames.Collation];
      if (collation != null)
      {
        const string collationClausePattern = @"COLLATE \w+";
        var collationClause = $@"COLLATE {collation}";

        columnType = Regex.IsMatch(columnType, collationClausePattern, RegexOptions.IgnoreCase)
            ? Regex.Replace(columnType, collationClausePattern, collationClause)
            : columnType.TrimEnd() + " " + collationClause;
      }

      return columnType;
    }

    protected override string GetColumnType(string schema, string table, string name, ColumnOperation operation, IModel model)
    => GetColumnTypeWithCharSetAndCollation(
        operation,
        base.GetColumnType(schema, table, name, operation, model),
        (model.GetRelationalModel().FindTable(table, schema)?.FindColumn(name) != null)
            ? FindProperty(model, schema, table, name) : null);
        //base.GetColumnType(schema, table, name, operation, model), FindProperty(model, schema, table, name));
        //base.GetColumnType(schema, table, name, operation, model));

    /// <summary>
    /// Generates a SQL fragment for the default constraint of a column.
    /// </summary>
    /// <param name="defaultValue"> The default value for the column. </param>
    /// <param name="defaultValueSql"> The SQL expression to use for the column's default constraint. </param>
    /// <param name="builder"> The command builder to use to add the SQL fragment. </param>
    protected override void DefaultValue(object defaultValue, string defaultValueSql, string columnType, MigrationCommandListBuilder builder)
    {
      Check.NotNull(builder, nameof(builder));

      if (defaultValueSql != null)
      {
        builder
            .Append(" DEFAULT ")
            .Append(defaultValueSql);
      }
      else if (defaultValue != null)
      {
        var typeMapping = Dependencies.TypeMappingSource.GetMappingForValue(defaultValue);
        builder
            .Append(" DEFAULT ")
            .Append(typeMapping.GenerateSqlLiteral(defaultValue));
      }
    }


    protected override void PrimaryKeyConstraint(
         [NotNull] AddPrimaryKeyOperation operation,
         [CanBeNull] IModel model,
         [NotNull] MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));


      //MySQL always assign PRIMARY to the PK name no way to override that.
      // check http://dev.mysql.com/doc/refman/5.1/en/create-table.html

      builder
          .Append("PRIMARY KEY ")
          .Append("(")
          .Append(string.Join(", ", operation.Columns.Select(Dependencies.SqlGenerationHelper.DelimitIdentifier)))
          .Append(")");

      IndexTraits(operation, model, builder);
    }

    protected override void Generate(AlterColumnOperation operation, IModel model, MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
       .Append("ALTER TABLE ")
       .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
       .Append(" MODIFY ");
      ColumnDefinition(operation.Schema, operation.Table, operation.Name, operation, model, builder);
      builder
        .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
      EndStatement(builder);
    }

    protected override void Generate(RenameTableOperation operation, IModel model, MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
      .Append("ALTER TABLE " + operation.Name)
      .Append(" RENAME " + operation.NewName)
      .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

      EndStatement(builder);
    }

    protected override void Generate(CreateIndexOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
      .Append("CREATE " + (operation.IsUnique ? "UNIQUE " : "") + "INDEX ");

      string schema = string.IsNullOrWhiteSpace(operation.Schema) ? string.Empty : $"`{operation.Schema}`.";

      builder.Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name) +
        $" ON {schema}`{operation.Table}` ({string.Join(", ", operation.Columns.Select(Dependencies.SqlGenerationHelper.DelimitIdentifier))})")
             .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);

      EndStatement(builder);
    }

    protected override void Generate(RenameIndexOperation operation, IModel model, MigrationCommandListBuilder builder)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder.Append("ALTER TABLE ")
        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Table, operation.Schema))
        .Append(" RENAME INDEX ")
        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.Name))
        .Append(" TO ")
        .Append(Dependencies.SqlGenerationHelper.DelimitIdentifier(operation.NewName))
        .AppendLine(";");

      EndStatement(builder);
    }

    protected override void Generate(DropIndexOperation operation, IModel model, MigrationCommandListBuilder builder, bool terminate)
    {
      Check.NotNull(operation, nameof(operation));
      Check.NotNull(builder, nameof(builder));

      builder
      .Append("DROP INDEX ")
      .Append(operation.Name)
      .Append(" ON " + operation.Table)
      .AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
      EndStatement(builder);
    }

    protected override void Generate(
      [NotNull] CreateTableOperation operation,
      [CanBeNull] IModel model,
      [NotNull] MigrationCommandListBuilder builder,
      bool terminate = true)
    {
      base.Generate(operation, model, builder, false);

      if (model.GetRelationalModel().FindTable(operation.Name, operation.Schema) !=null)
      {
        var entity = FindEntityTypes(model, operation.Schema, operation.Name).FirstOrDefault();
        var charset = entity?.FindAnnotation(MySQLAnnotationNames.Charset);
        if (charset != null)
        {
          builder.Append($" CHARACTER SET {charset.Value}");
        }

        var collation = entity?.FindAnnotation(MySQLAnnotationNames.Collation);
        if (collation != null)
        {
          builder.Append($" COLLATE {collation.Value}");
        }
      }

      if (terminate)
      {
        builder.AppendLine(Dependencies.SqlGenerationHelper.StatementTerminator);
        EndStatement(builder);
      }
    }

    protected override void Generate(
      DropPrimaryKeyOperation operation,
      IModel model,
      MigrationCommandListBuilder builder,
      bool terminate = true)
    {
      // It does nothing due to this operation should not be isolated for MySQL
      EndStatement(builder);
    }

    protected override void Generate(
      AddPrimaryKeyOperation operation,
      IModel model,
      MigrationCommandListBuilder builder,
      bool terminate = true)
    {
      // It does nothing due to this operation should not be isolated for MySQL
      EndStatement(builder);
    }
  }
}
