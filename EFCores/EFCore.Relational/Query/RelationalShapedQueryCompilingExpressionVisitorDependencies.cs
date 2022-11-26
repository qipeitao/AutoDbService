// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.EntityFrameworkCore.Query
{
    /// <summary>
    ///     <para>
    ///         Service dependencies parameter class for <see cref="RelationalShapedQueryCompilingExpressionVisitor" />
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    ///     <para>
    ///         Do not construct instances of this class directly from either provider or application code as the
    ///         constructor signature may change as new dependencies are added. Instead, use this type in
    ///         your constructor so that an instance will be created and injected automatically by the
    ///         dependency injection container. To create an instance with some dependent services replaced,
    ///         first resolve the object from the dependency injection container, then replace selected
    ///         services using the 'With...' methods. Do not call the constructor at any point in this process.
    ///     </para>
    ///     <para>
    ///         The service lifetime is <see cref="ServiceLifetime.Scoped" />. This means that each
    ///         <see cref="DbContext" /> instance will use its own instance of this service.
    ///         The implementation may depend on other services registered with any lifetime.
    ///         The implementation does not need to be thread-safe.
    ///     </para>
    /// </summary>
    public sealed class RelationalShapedQueryCompilingExpressionVisitorDependencies
    {
        /// <summary>
        ///     <para>
        ///         Creates the service dependencies parameter object for a <see cref="RelationalShapedQueryCompilingExpressionVisitor" />.
        ///     </para>
        ///     <para>
        ///         Do not call this constructor directly from either provider or application code as it may change
        ///         as new dependencies are added. Instead, use this type in your constructor so that an instance
        ///         will be created and injected automatically by the dependency injection container. To create
        ///         an instance with some dependent services replaced, first resolve the object from the dependency
        ///         injection container, then replace selected services using the 'With...' methods. Do not call
        ///         the constructor at any point in this process.
        ///     </para>
        ///     <para>
        ///         This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///         the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///         any release. You should only use it directly in your code with extreme caution and knowing that
        ///         doing so can result in application failures when updating to a new Entity Framework Core release.
        ///     </para>
        /// </summary>
        [EntityFrameworkInternal]
        public RelationalShapedQueryCompilingExpressionVisitorDependencies(
            [NotNull] IQuerySqlGeneratorFactory querySqlGeneratorFactory,
            [NotNull] ISqlExpressionFactory sqlExpressionFactory,
            [NotNull] IParameterNameGeneratorFactory parameterNameGeneratorFactory,
            [NotNull] IRelationalParameterBasedSqlProcessorFactory relationalParameterBasedSqlProcessorFactory,
            [NotNull] ICoreSingletonOptions coreSingletonOptions)
        {
            Check.NotNull(querySqlGeneratorFactory, nameof(querySqlGeneratorFactory));
            Check.NotNull(sqlExpressionFactory, nameof(sqlExpressionFactory));
            Check.NotNull(parameterNameGeneratorFactory, nameof(parameterNameGeneratorFactory));
            Check.NotNull(relationalParameterBasedSqlProcessorFactory, nameof(relationalParameterBasedSqlProcessorFactory));

            QuerySqlGeneratorFactory = querySqlGeneratorFactory;
#pragma warning disable CS0618 // Type or member is obsolete
            SqlExpressionFactory = sqlExpressionFactory;
            ParameterNameGeneratorFactory = parameterNameGeneratorFactory;
#pragma warning restore CS0618 // Type or member is obsolete
            RelationalParameterBasedSqlProcessorFactory = relationalParameterBasedSqlProcessorFactory;
            CoreSingletonOptions = coreSingletonOptions;
        }

        /// <summary>
        ///     The SQL generator factory.
        /// </summary>
        public IQuerySqlGeneratorFactory QuerySqlGeneratorFactory { get; }

        /// <summary>
        ///     The SQL expression factory.
        /// </summary>
        [Obsolete("Use the service from " + nameof(RelationalParameterBasedSqlProcessorDependencies) + ".")]
        public ISqlExpressionFactory SqlExpressionFactory { get; }

        /// <summary>
        ///     The parameter name-generator factory.
        /// </summary>
        [Obsolete("Use the service from " + nameof(RelationalParameterBasedSqlProcessorDependencies) + ".")]
        public IParameterNameGeneratorFactory ParameterNameGeneratorFactory { get; }

        /// <summary>
        ///     The SQL processor based on parameter values.
        /// </summary>
        public IRelationalParameterBasedSqlProcessorFactory RelationalParameterBasedSqlProcessorFactory { get; }

        /// <summary>
        ///     Core singleton options.
        /// </summary>
        public ICoreSingletonOptions CoreSingletonOptions { get; }

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="querySqlGeneratorFactory"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalShapedQueryCompilingExpressionVisitorDependencies With(
            [NotNull] IQuerySqlGeneratorFactory querySqlGeneratorFactory)
            => new RelationalShapedQueryCompilingExpressionVisitorDependencies(
                querySqlGeneratorFactory,
#pragma warning disable CS0618 // Type or member is obsolete
                SqlExpressionFactory,
                ParameterNameGeneratorFactory,
#pragma warning restore CS0618 // Type or member is obsolete
                RelationalParameterBasedSqlProcessorFactory,
                CoreSingletonOptions);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="sqlExpressionFactory"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalShapedQueryCompilingExpressionVisitorDependencies With([NotNull] ISqlExpressionFactory sqlExpressionFactory)
            => new RelationalShapedQueryCompilingExpressionVisitorDependencies(
                QuerySqlGeneratorFactory,
                sqlExpressionFactory,
#pragma warning disable CS0618 // Type or member is obsolete
                ParameterNameGeneratorFactory,
#pragma warning restore CS0618 // Type or member is obsolete
                RelationalParameterBasedSqlProcessorFactory,
                CoreSingletonOptions);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="parameterNameGeneratorFactory"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalShapedQueryCompilingExpressionVisitorDependencies With(
            [NotNull] IParameterNameGeneratorFactory parameterNameGeneratorFactory)
            => new RelationalShapedQueryCompilingExpressionVisitorDependencies(
                QuerySqlGeneratorFactory,
#pragma warning disable CS0618 // Type or member is obsolete
                SqlExpressionFactory,
#pragma warning restore CS0618 // Type or member is obsolete
                parameterNameGeneratorFactory,
                RelationalParameterBasedSqlProcessorFactory,
                CoreSingletonOptions);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="relationalParameterBasedSqlProcessorFactory"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalShapedQueryCompilingExpressionVisitorDependencies With(
            [NotNull] IRelationalParameterBasedSqlProcessorFactory relationalParameterBasedSqlProcessorFactory)
            => new RelationalShapedQueryCompilingExpressionVisitorDependencies(
                QuerySqlGeneratorFactory,
#pragma warning disable CS0618 // Type or member is obsolete
                SqlExpressionFactory,
                ParameterNameGeneratorFactory,
#pragma warning restore CS0618 // Type or member is obsolete
                relationalParameterBasedSqlProcessorFactory,
                CoreSingletonOptions);

        /// <summary>
        ///     Clones this dependency parameter object with one service replaced.
        /// </summary>
        /// <param name="coreSingletonOptions"> A replacement for the current dependency of this type. </param>
        /// <returns> A new parameter object with the given service replaced. </returns>
        public RelationalShapedQueryCompilingExpressionVisitorDependencies With(
            [NotNull] ICoreSingletonOptions coreSingletonOptions)
            => new RelationalShapedQueryCompilingExpressionVisitorDependencies(
                QuerySqlGeneratorFactory,
#pragma warning disable CS0618 // Type or member is obsolete
                SqlExpressionFactory,
                ParameterNameGeneratorFactory,
#pragma warning restore CS0618 // Type or member is obsolete
                RelationalParameterBasedSqlProcessorFactory,
                coreSingletonOptions);
    }
}
