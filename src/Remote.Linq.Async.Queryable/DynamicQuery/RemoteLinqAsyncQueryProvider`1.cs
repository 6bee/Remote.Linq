// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using Aqua.TypeExtensions;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using MethodInfo = System.Reflection.MethodInfo;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    public class RemoteLinqAsyncQueryProvider<TSource> : IRemoteLinqAsyncQueryProvider
    {
        private static readonly MethodInfo _mapAsyncEnumerableMethodInfo =
            typeof(RemoteLinqAsyncQueryProvider<TSource>).GetMethodEx(nameof(MapAsyncEnumerableInternal));

        private static readonly MethodInfo _mapSingleElementStreamMethodInfo =
            typeof(RemoteLinqAsyncQueryProvider<TSource>).GetMethodEx(nameof(MapSingleElementStreamInternal));

        private readonly Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>>? _asyncStreamProvider;
        private readonly Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>>? _asyncDataProvider;
        private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
        private readonly IExpressionToRemoteLinqContext? _context;

        [SecuritySafeCritical]
        public RemoteLinqAsyncQueryProvider(
            Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource?>>? asyncStreamProvider,
            Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>>? asyncDataProvider,
            IAsyncQueryResultMapper<TSource> resultMapper,
            IExpressionToRemoteLinqContext? context = null)
        {
            if (asyncStreamProvider is null && asyncDataProvider is null)
            {
                throw new ArgumentNullException(
                    $"{nameof(asyncStreamProvider)}, {nameof(asyncDataProvider)}",
                    $"At least one of '{nameof(asyncStreamProvider)}' and '{nameof(asyncDataProvider)}' must not be null.");
            }

            _asyncStreamProvider = asyncStreamProvider;
            _asyncDataProvider = asyncDataProvider;
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _context = context;
        }

        public IAsyncQueryable<TElement> CreateQuery<TElement>(SystemLinq.Expression expression)
             => new RemoteLinqAsyncQueryable<TElement>(this, expression);

        public async ValueTask<TResult> ExecuteAsync<TResult>(SystemLinq.Expression expression, CancellationToken cancellation)
        {
            if (typeof(TResult).Implements(typeof(IAsyncEnumerable<>), out var genericArguments))
            {
                var resultItemType = genericArguments[0];

                ExpressionHelper.CheckExpressionResultType<TResult>(expression);

                cancellation.ThrowIfCancellationRequested();
                var rlinq = ExpressionHelper.TranslateExpression(expression, _context);

                cancellation.ThrowIfCancellationRequested();

                if (_asyncStreamProvider is not null)
                {
                    var transferItemStream = _asyncStreamProvider(rlinq, cancellation);
                    var mappedResultStream = MapAsyncEnumerable<TResult>(resultItemType, transferItemStream, cancellation);
                    return mappedResultStream;
                }

                var transferRecords = await _asyncDataProvider!(rlinq, cancellation).ConfigureAwait(false);
                var mappedResult = await _resultMapper.MapResultAsync<TResult>(transferRecords, expression, cancellation).ConfigureAwait(false);
                return mappedResult;
            }
            else
            {
                ExpressionHelper.CheckExpressionResultType<ValueTask<TResult>>(expression);

                cancellation.ThrowIfCancellationRequested();
                var rlinq = ExpressionHelper.TranslateExpression(expression, _context);

                if (_asyncDataProvider is not null)
                {
                    var transferData = await _asyncDataProvider(rlinq, cancellation).ConfigureAwait(false);
                    var mappedResult = await _resultMapper.MapResultAsync<TResult>(transferData, expression, cancellation).ConfigureAwait(false);
                    return mappedResult;
                }

                var transferStream = _asyncStreamProvider!(rlinq, cancellation);
                var mappedStreamResult = await MapSingleElementStream<TResult>(transferStream, cancellation).ConfigureAwait(false);
                return mappedStreamResult;
            }
        }

        private TResult MapAsyncEnumerable<TResult>(Type elementType, IAsyncEnumerable<TSource?> source, CancellationToken cancellation)
        {
            var method = _mapAsyncEnumerableMethodInfo.MakeGenericMethod(elementType);
            var result = method.Invoke(null, new object[] { source, _resultMapper, cancellation });
            return (TResult)result!;
        }

        private ValueTask<TResult> MapSingleElementStream<TResult>(IAsyncEnumerable<TSource?> source, CancellationToken cancellation)
        {
            var method = _mapSingleElementStreamMethodInfo.MakeGenericMethod(typeof(TResult));
            var result = method.Invoke(null, new object[] { source, _resultMapper, cancellation });
            return (ValueTask<TResult>)result!;
        }

        private static async IAsyncEnumerable<T> MapAsyncEnumerableInternal<T>(IAsyncEnumerable<TSource?> source, IAsyncQueryResultMapper<TSource?> resultMapper, [EnumeratorCancellation] CancellationToken cancellation)
        {
            await foreach (var item in source.WithCancellation(cancellation).ConfigureAwait(false))
            {
                yield return await resultMapper.MapResultAsync<T>(item, null!).ConfigureAwait(false);
            }
        }

        private static async ValueTask<T> MapSingleElementStreamInternal<T>(IAsyncEnumerable<TSource?> source, IAsyncQueryResultMapper<TSource?> resultMapper, CancellationToken cancellation)
        {
            var asyncEnumerator = source.GetAsyncEnumerator(cancellation);

            if (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false) is false)
            {
                throw new RemoteLinqException("Remote stream returned no data");
            }

            var result = await resultMapper.MapResultAsync<T>(asyncEnumerator.Current, null!).ConfigureAwait(false);

            if (await asyncEnumerator.MoveNextAsync().ConfigureAwait(false))
            {
                throw new RemoteLinqException("Remote stream returned more than one element");
            }

            return result;
        }
    }
}