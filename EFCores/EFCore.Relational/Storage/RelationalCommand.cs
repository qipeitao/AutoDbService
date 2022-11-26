// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Utilities;

namespace Microsoft.EntityFrameworkCore.Storage
{
    /// <summary>
    ///     <para>
    ///         A command to be executed against a relational database.
    ///     </para>
    ///     <para>
    ///         This type is typically used by database providers (and other extensions). It is generally
    ///         not used in application code.
    ///     </para>
    /// </summary>
    public class RelationalCommand : IRelationalCommand
    {
        /// <summary>
        ///     <para>
        ///         Constructs a new <see cref="RelationalCommand" />.
        ///     </para>
        ///     <para>
        ///         This type is typically used by database providers (and other extensions). It is generally
        ///         not used in application code.
        ///     </para>
        /// </summary>
        /// <param name="dependencies"> Service dependencies. </param>
        /// <param name="commandText"> The text of the command to be executed. </param>
        /// <param name="parameters"> Parameters for the command. </param>
        public RelationalCommand(
            [NotNull] RelationalCommandBuilderDependencies dependencies,
            [NotNull] string commandText,
            [NotNull] IReadOnlyList<IRelationalParameter> parameters)
        {
            Check.NotNull(dependencies, nameof(dependencies));
            Check.NotNull(commandText, nameof(commandText));
            Check.NotNull(parameters, nameof(parameters));

            Dependencies = dependencies;
            CommandText = commandText;
            Parameters = parameters;
        }

        /// <summary>
        ///     Command building dependencies.
        /// </summary>
        protected virtual RelationalCommandBuilderDependencies Dependencies { get; }

        /// <summary>
        ///     Gets the command text to be executed.
        /// </summary>
        public virtual string CommandText { get; }

        /// <summary>
        ///     Gets the parameters for the command.
        /// </summary>
        public virtual IReadOnlyList<IRelationalParameter> Parameters { get; }

        /// <summary>
        ///     Executes the command with no results.
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <returns> The number of rows affected. </returns>
        public virtual int ExecuteNonQuery(RelationalCommandParameterObject parameterObject)
        {
            var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);

            var commandId = Guid.NewGuid();
            var command = CreateDbCommand(parameterObject, commandId, DbCommandMethod.ExecuteNonQuery);

            connection.Open();

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var interceptionResult = logger?.CommandNonQueryExecuting(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        startTime)
                    ?? default;

                var nonQueryResult = interceptionResult.HasResult
                    ? interceptionResult.Result
                    : command.ExecuteNonQuery();

                return logger?.CommandNonQueryExecuted(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        nonQueryResult,
                        startTime,
                        stopwatch.Elapsed)
                    ?? nonQueryResult;
            }
            catch (Exception exception)
            {
                logger?.CommandError(
                    connection,
                    command,
                    context,
                    DbCommandMethod.ExecuteNonQuery,
                    commandId,
                    connection.ConnectionId,
                    exception,
                    startTime,
                    stopwatch.Elapsed);

                throw;
            }
            finally
            {
                CleanupCommand(command, connection);
            }
        }

        private static void CleanupCommand(
            DbCommand command,
            IRelationalConnection connection)
        {
            command.Parameters.Clear();
            command.Dispose();
            connection.Close();
        }

        private static async Task CleanupCommandAsync(
            DbCommand command,
            IRelationalConnection connection)
        {
            command.Parameters.Clear();
            await command.DisposeAsync().ConfigureAwait(false);
            await connection.CloseAsync().ConfigureAwait(false);
        }

        /// <summary>
        ///     Asynchronously executes the command with no results.
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the number of rows affected.
        /// </returns>
        public virtual async Task<int> ExecuteNonQueryAsync(
            RelationalCommandParameterObject parameterObject,
            CancellationToken cancellationToken = default)
        {
            var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);

            var commandId = Guid.NewGuid();
            var command = CreateDbCommand(parameterObject, commandId, DbCommandMethod.ExecuteNonQuery);

            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var interceptionResult = logger == null
                    ? default
                    : await logger.CommandNonQueryExecutingAsync(
                            connection,
                            command,
                            context,
                            commandId,
                            connection.ConnectionId,
                            startTime,
                            cancellationToken)
                        .ConfigureAwait(false);

                var result = interceptionResult.HasResult
                    ? interceptionResult.Result
                    : await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

                if (logger != null)
                {
                    result = await logger.CommandNonQueryExecutedAsync(
                            connection,
                            command,
                            context,
                            commandId,
                            connection.ConnectionId,
                            result,
                            startTime,
                            stopwatch.Elapsed,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception exception)
            {
                if (logger != null)
                {
                    await logger.CommandErrorAsync(
                            connection,
                            command,
                            context,
                            DbCommandMethod.ExecuteNonQuery,
                            commandId,
                            connection.ConnectionId,
                            exception,
                            startTime,
                            stopwatch.Elapsed,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                throw;
            }
            finally
            {
                await CleanupCommandAsync(command, connection).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Executes the command with a single scalar result.
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <returns> The result of the command. </returns>
        public virtual object ExecuteScalar(RelationalCommandParameterObject parameterObject)
        {
            var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);

            var commandId = Guid.NewGuid();
            var command = CreateDbCommand(parameterObject, commandId, DbCommandMethod.ExecuteScalar);

            connection.Open();

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();
            try
            {
                var interceptionResult = logger?.CommandScalarExecuting(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        startTime)
                    ?? default;

                var result = interceptionResult.HasResult
                    ? interceptionResult.Result
                    : command.ExecuteScalar();

                return logger?.CommandScalarExecuted(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        result,
                        startTime,
                        stopwatch.Elapsed)
                    ?? result;
            }
            catch (Exception exception)
            {
                logger?.CommandError(
                    connection,
                    command,
                    context,
                    DbCommandMethod.ExecuteScalar,
                    commandId,
                    connection.ConnectionId,
                    exception,
                    startTime,
                    stopwatch.Elapsed);

                throw;
            }
            finally
            {
                CleanupCommand(command, connection);
            }
        }

        /// <summary>
        ///     Asynchronously executes the command with a single scalar result.
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the result of the command.
        /// </returns>
        public virtual async Task<object> ExecuteScalarAsync(
            RelationalCommandParameterObject parameterObject,
            CancellationToken cancellationToken = default)
        {
            var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);

            var commandId = Guid.NewGuid();
            var command = CreateDbCommand(parameterObject, commandId, DbCommandMethod.ExecuteScalar);

            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            try
            {
                var interceptionResult = logger == null
                    ? default
                    : await logger.CommandScalarExecutingAsync(
                            connection,
                            command,
                            context,
                            commandId,
                            connection.ConnectionId,
                            startTime,
                            cancellationToken)
                        .ConfigureAwait(false);

                var result = interceptionResult.HasResult
                    ? interceptionResult.Result
                    : await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);

                if (logger != null)
                {
                    result = await logger.CommandScalarExecutedAsync(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        result,
                        startTime,
                        stopwatch.Elapsed,
                        cancellationToken).ConfigureAwait(false);
                }

                return result;
            }
            catch (Exception exception)
            {
                if (logger != null)
                {
                    await logger.CommandErrorAsync(
                            connection,
                            command,
                            context,
                            DbCommandMethod.ExecuteScalar,
                            commandId,
                            connection.ConnectionId,
                            exception,
                            startTime,
                            stopwatch.Elapsed,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                throw;
            }
            finally
            {
                await CleanupCommandAsync(command, connection).ConfigureAwait(false);
            }
        }

        /// <summary>
        ///     Executes the command with a <see cref="RelationalDataReader" /> result.
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <returns> The result of the command. </returns>
        public virtual RelationalDataReader ExecuteReader(RelationalCommandParameterObject parameterObject)
        {
            var connection = parameterObject.Connection;
            var context = parameterObject.Context;
            var readerColumns = parameterObject.ReaderColumns;
            var logger = parameterObject.Logger;
            var detailedErrorsEnabled = parameterObject.DetailedErrorsEnabled;

            var commandId = Guid.NewGuid();
            var command = CreateDbCommand(parameterObject, commandId, DbCommandMethod.ExecuteReader);

            connection.Open();

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            var readerOpen = false;
            DbDataReader reader;
            try
            {
                var interceptionResult = logger?.CommandReaderExecuting(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        startTime)
                    ?? default;

                reader = interceptionResult.HasResult
                    ? interceptionResult.Result
                    : command.ExecuteReader();

                if (logger != null)
                {
                    reader = logger.CommandReaderExecuted(
                        connection,
                        command,
                        context,
                        commandId,
                        connection.ConnectionId,
                        reader,
                        startTime,
                        stopwatch.Elapsed);
                }
            }
            catch (Exception exception)
            {
                logger?.CommandError(
                    connection,
                    command,
                    context,
                    DbCommandMethod.ExecuteReader,
                    commandId,
                    connection.ConnectionId,
                    exception,
                    startTime,
                    stopwatch.Elapsed);

                CleanupCommand(command, connection);

                throw;
            }

            try
            {
                if (readerColumns != null)
                {
                    reader = new BufferedDataReader(reader, detailedErrorsEnabled).Initialize(readerColumns);
                }

                var result = CreateRelationalDataReader(
                    connection,
                    command,
                    reader,
                    commandId,
                    logger);

                readerOpen = true;

                return result;
            }
            finally
            {
                if (!readerOpen)
                {
                    CleanupCommand(command, connection);
                }
            }
        }

        /// <summary>
        ///     Asynchronously executes the command with a <see cref="RelationalDataReader" /> result.
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <param name="cancellationToken">A <see cref="CancellationToken" /> to observe while waiting for the task to complete.</param>
        /// <returns>
        ///     A task that represents the asynchronous operation. The task result contains the result of the command.
        /// </returns>
        public virtual async Task<RelationalDataReader> ExecuteReaderAsync(
            RelationalCommandParameterObject parameterObject,
            CancellationToken cancellationToken = default)
        {
            var connection = parameterObject.Connection;
            var context = parameterObject.Context;
            var readerColumns = parameterObject.ReaderColumns;
            var logger = parameterObject.Logger;
            var detailedErrorsEnabled = parameterObject.DetailedErrorsEnabled;

            var commandId = Guid.NewGuid();
            var command = CreateDbCommand(parameterObject, commandId, DbCommandMethod.ExecuteReader);

            await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            var readerOpen = false;
            DbDataReader reader;
            try
            {
                var interceptionResult = logger == null
                    ? default
                    : await logger.CommandReaderExecutingAsync(
                            connection,
                            command,
                            context,
                            commandId,
                            connection.ConnectionId,
                            startTime,
                            cancellationToken)
                        .ConfigureAwait(false);

                reader = interceptionResult.HasResult
                    ? interceptionResult.Result
                    : await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

                if (logger != null)
                {
                    reader = await logger.CommandReaderExecutedAsync(
                            connection,
                            command,
                            context,
                            commandId,
                            connection.ConnectionId,
                            reader,
                            startTime,
                            stopwatch.Elapsed,
                            cancellationToken)
                        .ConfigureAwait(false);
                }
            }
            catch (Exception exception)
            {
                if (logger != null)
                {
                    await logger.CommandErrorAsync(
                            connection,
                            command,
                            context,
                            DbCommandMethod.ExecuteReader,
                            commandId,
                            connection.ConnectionId,
                            exception,
                            startTime,
                            stopwatch.Elapsed,
                            cancellationToken)
                        .ConfigureAwait(false);
                }

                await CleanupCommandAsync(command, connection).ConfigureAwait(false);

                throw;
            }

            try
            {
                if (readerColumns != null)
                {
                    reader = await new BufferedDataReader(reader, detailedErrorsEnabled).InitializeAsync(readerColumns, cancellationToken)
                        .ConfigureAwait(false);
                }

                var result = CreateRelationalDataReader(
                    connection,
                    command,
                    reader,
                    commandId,
                    logger);

                readerOpen = true;

                return result;

            }
            finally
            {
                if (!readerOpen)
                {
                    await CleanupCommandAsync(command, connection).ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        ///     <para>
        ///         Called by the execute methods to
        ///         create a <see cref="DbCommand" /> for the given <see cref="DbConnection" /> and configure
        ///         timeouts and transactions.
        ///     </para>
        ///     <para>
        ///         This method is typically used by database providers (and other extensions). It is generally
        ///         not used in application code.
        ///     </para>
        /// </summary>
        /// <param name="parameterObject"> Parameters for this method. </param>
        /// <param name="commandId"> The command correlation ID. </param>
        /// <param name="commandMethod"> The method that will be called on the created command. </param>
        /// <returns> The created command. </returns>
        public virtual DbCommand CreateDbCommand(
            RelationalCommandParameterObject parameterObject,
            Guid commandId,
            DbCommandMethod commandMethod)
        {
            var (connection, context, logger) = (parameterObject.Connection, parameterObject.Context, parameterObject.Logger);
            var connectionId = connection.ConnectionId;

            var startTime = DateTimeOffset.UtcNow;
            var stopwatch = Stopwatch.StartNew();

            var interceptionResult = logger?.CommandCreating(connection, commandMethod, context, commandId, connectionId, startTime)
                ?? default;

            var command = interceptionResult.HasResult
                ? interceptionResult.Result
                : connection.DbConnection.CreateCommand();

            if (logger != null)
            {
                command = logger.CommandCreated(
                    connection, command, commandMethod, context, commandId, connectionId, startTime, stopwatch.Elapsed);
            }

            command.CommandText = CommandText;

            if (connection.CurrentTransaction != null)
            {
                command.Transaction = connection.CurrentTransaction.GetDbTransaction();
            }

            if (connection.CommandTimeout != null)
            {
                command.CommandTimeout = (int)connection.CommandTimeout;
            }

            if (Parameters != null
                && Parameters.Count > 0)
            {
                var parameterValues = parameterObject.ParameterValues;
                if (parameterValues == null)
                {
                    throw new InvalidOperationException(
                        RelationalStrings.MissingParameterValue(
                            Parameters[0].InvariantName));
                }

                for (var i = 0; i < Parameters.Count; i++)
                {
                    Parameters[i].AddDbParameter(command, parameterValues);
                }
            }

            return command;
        }

        /// <summary>
        ///     <para>
        ///         Creates a new <see cref="RelationalDataReader" /> to be used by <see cref="ExecuteReader" /> and <see cref="ExecuteReaderAsync" />.
        ///     </para>
        ///     <para>
        ///         This method is typically used by database providers (and other extensions). It is generally
        ///         not used in application code.
        ///     </para>
        /// </summary>
        /// <param name="connection">The connection, to pass to the <see cref="RelationalDataReader" /> constructor.</param>
        /// <param name="command">The command that was executed, to pass to the <see cref="RelationalDataReader" /> constructor.</param>
        /// <param name="reader">The underlying reader for the result set, to pass to the <see cref="RelationalDataReader" /> constructor.</param>
        /// <param name="commandId">
        ///     A correlation ID that identifies the <see cref="DbCommand" /> instance being used, to pass to the
        ///     <see cref="RelationalDataReader" /> constructor.
        /// </param>
        /// <param name="logger">The diagnostic source, to pass to the <see cref="RelationalDataReader" /> constructor.</param>
        /// <returns>The created <see cref="RelationalDataReader" />.</returns>
        protected virtual RelationalDataReader CreateRelationalDataReader(
            [NotNull] IRelationalConnection connection,
            [NotNull] DbCommand command,
            [NotNull] DbDataReader reader,
            Guid commandId,
            [CanBeNull] IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger)
            => new RelationalDataReader(
                connection,
                command,
                reader,
                commandId,
                logger);
    }
}
