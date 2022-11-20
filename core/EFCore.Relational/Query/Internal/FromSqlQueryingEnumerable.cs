// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Microsoft.EntityFrameworkCore.Query.Internal
{
    /// <summary>
    ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
    ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
    ///     any release. You should only use it directly in your code with extreme caution and knowing that
    ///     doing so can result in application failures when updating to a new Entity Framework Core release.
    /// </summary>
    public class FromSqlQueryingEnumerable<T> : IEnumerable<T>, IAsyncEnumerable<T>, IRelationalQueryingEnumerable
    {
        private readonly RelationalQueryContext _relationalQueryContext;
        private readonly RelationalCommandCache _relationalCommandCache;
        private readonly IReadOnlyList<string> _columnNames;
        private readonly Func<QueryContext, DbDataReader, int[], T> _shaper;
        private readonly Type _contextType;
        private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
        private readonly bool _standAloneStateManager;
        private readonly bool _detailedErrorsEnabled;

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public FromSqlQueryingEnumerable(
            [NotNull] RelationalQueryContext relationalQueryContext,
            [NotNull] RelationalCommandCache relationalCommandCache,
            [NotNull] IReadOnlyList<string> columnNames,
            [NotNull] Func<QueryContext, DbDataReader, int[], T> shaper,
            [NotNull] Type contextType,
            bool standAloneStateManager,
            bool detailedErrorsEnabled)
        {
            _relationalQueryContext = relationalQueryContext;
            _relationalCommandCache = relationalCommandCache;
            _columnNames = columnNames;
            _shaper = shaper;
            _contextType = contextType;
            _queryLogger = relationalQueryContext.QueryLogger;
            _standAloneStateManager = standAloneStateManager;
            _detailedErrorsEnabled = detailedErrorsEnabled;
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            _relationalQueryContext.CancellationToken = cancellationToken;

            return new AsyncEnumerator(this);
        }

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual IEnumerator<T> GetEnumerator()
            => new Enumerator(this);

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual DbCommand CreateDbCommand()
            => _relationalCommandCache
                .GetRelationalCommand(_relationalQueryContext.ParameterValues)
                .CreateDbCommand(
                    new RelationalCommandParameterObject(
                        _relationalQueryContext.Connection,
                        _relationalQueryContext.ParameterValues,
                        null,
                        null,
                        null,
                        _detailedErrorsEnabled),
                    Guid.Empty,
                    (DbCommandMethod)(-1));

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public virtual string ToQueryString()
            => _relationalQueryContext.RelationalQueryStringFactory.Create(CreateDbCommand());

        /// <summary>
        ///     This is an internal API that supports the Entity Framework Core infrastructure and not subject to
        ///     the same compatibility standards as public APIs. It may be changed or removed without notice in
        ///     any release. You should only use it directly in your code with extreme caution and knowing that
        ///     doing so can result in application failures when updating to a new Entity Framework Core release.
        /// </summary>
        public static int[] BuildIndexMap([CanBeNull] IReadOnlyList<string> columnNames, [NotNull] DbDataReader dataReader)
        {
            var readerColumns = Enumerable.Range(0, dataReader.FieldCount)
                .ToDictionary(dataReader.GetName, i => i, StringComparer.OrdinalIgnoreCase);

            var indexMap = new int[columnNames.Count];
            for (var i = 0; i < columnNames.Count; i++)
            {
                var columnName = columnNames[i];
                if (!readerColumns.TryGetValue(columnName, out var ordinal))
                {
                    throw new InvalidOperationException(RelationalStrings.FromSqlMissingColumn(columnName));
                }

                indexMap[i] = ordinal;
            }

            return indexMap;
        }

        private sealed class Enumerator : IEnumerator<T>
        {
            private readonly RelationalQueryContext _relationalQueryContext;
            private readonly RelationalCommandCache _relationalCommandCache;
            private readonly IReadOnlyList<string> _columnNames;
            private readonly Func<QueryContext, DbDataReader, int[], T> _shaper;
            private readonly Type _contextType;
            private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
            private readonly bool _standAloneStateManager;
            private readonly bool _detailedErrorsEnabled;

            private RelationalDataReader _dataReader;
            private int[] _indexMap;

            public Enumerator(FromSqlQueryingEnumerable<T> queryingEnumerable)
            {
                _relationalQueryContext = queryingEnumerable._relationalQueryContext;
                _relationalCommandCache = queryingEnumerable._relationalCommandCache;
                _columnNames = queryingEnumerable._columnNames;
                _shaper = queryingEnumerable._shaper;
                _contextType = queryingEnumerable._contextType;
                _queryLogger = queryingEnumerable._queryLogger;
                _standAloneStateManager = queryingEnumerable._standAloneStateManager;
                _detailedErrorsEnabled = queryingEnumerable._detailedErrorsEnabled;
            }

            public T Current { get; private set; }

            object IEnumerator.Current
                => Current;

            public bool MoveNext()
            {
                try
                {
                    using (_relationalQueryContext.ConcurrencyDetector.EnterCriticalSection())
                    {
                        if (_dataReader == null)
                        {
                            _relationalQueryContext.ExecutionStrategyFactory.Create()
                                .Execute(true, InitializeReader, null);
                        }

                        var hasNext = _dataReader.Read();

                        Current = hasNext
                            ? _shaper(_relationalQueryContext, _dataReader.DbDataReader, _indexMap)
                            : default;

                        return hasNext;
                    }
                }
                catch (Exception exception)
                {
                    _queryLogger.QueryIterationFailed(_contextType, exception);

                    throw;
                }
            }

            private bool InitializeReader(DbContext _, bool result)
            {
                EntityFrameworkEventSource.Log.QueryExecuting();

                var relationalCommand = _relationalCommandCache.GetRelationalCommand(_relationalQueryContext.ParameterValues);

                _dataReader = relationalCommand.ExecuteReader(
                    new RelationalCommandParameterObject(
                        _relationalQueryContext.Connection,
                        _relationalQueryContext.ParameterValues,
                        _relationalCommandCache.ReaderColumns,
                        _relationalQueryContext.Context,
                        _relationalQueryContext.CommandLogger,
                        _detailedErrorsEnabled));

                _indexMap = BuildIndexMap(_columnNames, _dataReader.DbDataReader);

                _relationalQueryContext.InitializeStateManager(_standAloneStateManager);

                return result;
            }

            public void Dispose()
            {
                _dataReader?.Dispose();
                _dataReader = null;
            }

            public void Reset()
                => throw new NotImplementedException();
        }

        private sealed class AsyncEnumerator : IAsyncEnumerator<T>
        {
            private readonly RelationalQueryContext _relationalQueryContext;
            private readonly RelationalCommandCache _relationalCommandCache;
            private readonly IReadOnlyList<string> _columnNames;
            private readonly Func<QueryContext, DbDataReader, int[], T> _shaper;
            private readonly Type _contextType;
            private readonly IDiagnosticsLogger<DbLoggerCategory.Query> _queryLogger;
            private readonly bool _standAloneStateManager;
            private readonly bool _detailedErrorsEnabled;

            private RelationalDataReader _dataReader;
            private int[] _indexMap;

            public AsyncEnumerator(FromSqlQueryingEnumerable<T> queryingEnumerable)
            {
                _relationalQueryContext = queryingEnumerable._relationalQueryContext;
                _relationalCommandCache = queryingEnumerable._relationalCommandCache;
                _columnNames = queryingEnumerable._columnNames;
                _shaper = queryingEnumerable._shaper;
                _contextType = queryingEnumerable._contextType;
                _queryLogger = queryingEnumerable._queryLogger;
                _standAloneStateManager = queryingEnumerable._standAloneStateManager;
                _detailedErrorsEnabled = queryingEnumerable._detailedErrorsEnabled;
            }

            public T Current { get; private set; }

            public async ValueTask<bool> MoveNextAsync()
            {
                try
                {
                    using (_relationalQueryContext.ConcurrencyDetector.EnterCriticalSection())
                    {
                        if (_dataReader == null)
                        {
                            await _relationalQueryContext.ExecutionStrategyFactory.Create()
                                .ExecuteAsync(true, InitializeReaderAsync, null, _relationalQueryContext.CancellationToken).ConfigureAwait(false);
                        }

                        var hasNext = await _dataReader.ReadAsync(_relationalQueryContext.CancellationToken).ConfigureAwait(false);

                        Current = hasNext
                            ? _shaper(_relationalQueryContext, _dataReader.DbDataReader, _indexMap)
                            : default;

                        return hasNext;
                    }
                }
                catch (Exception exception)
                {
                    _queryLogger.QueryIterationFailed(_contextType, exception);

                    throw;
                }
            }

            private async Task<bool> InitializeReaderAsync(DbContext _, bool result, CancellationToken cancellationToken)
            {
                EntityFrameworkEventSource.Log.QueryExecuting();

                var relationalCommand = _relationalCommandCache.GetRelationalCommand(_relationalQueryContext.ParameterValues);

                _dataReader = await relationalCommand.ExecuteReaderAsync(
                    new RelationalCommandParameterObject(
                        _relationalQueryContext.Connection,
                        _relationalQueryContext.ParameterValues,
                        _relationalCommandCache.ReaderColumns,
                        _relationalQueryContext.Context,
                        _relationalQueryContext.CommandLogger,
                        _detailedErrorsEnabled),
                    cancellationToken)
                    .ConfigureAwait(false);

                _indexMap = BuildIndexMap(_columnNames, _dataReader.DbDataReader);

                _relationalQueryContext.InitializeStateManager(_standAloneStateManager);

                return result;
            }

            public ValueTask DisposeAsync()
            {
                if (_dataReader != null)
                {
                    var dataReader = _dataReader;
                    _dataReader = null;

                    return dataReader.DisposeAsync();
                }

                return default;
            }
        }
    }
}
