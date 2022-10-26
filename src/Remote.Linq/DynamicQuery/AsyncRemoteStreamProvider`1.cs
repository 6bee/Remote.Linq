// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;

    public sealed class AsyncRemoteStreamProvider<TSource> : IAsyncRemoteStreamProvider
    {
        private readonly Func<Expressions.Expression, CancellationToken, IAsyncEnumerable<TSource?>> _dataProvider;
        private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
        private readonly IExpressionToRemoteLinqContext _context;

        [SecuritySafeCritical]
        public AsyncRemoteStreamProvider(
            Func<Expressions.Expression, CancellationToken, IAsyncEnumerable<TSource?>> dataProvider,
            IExpressionToRemoteLinqContext? context,
            IAsyncQueryResultMapper<TSource> resultMapper)
        {
            _dataProvider = dataProvider.CheckNotNull(nameof(dataProvider));
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _context = context ?? ExpressionTranslatorContext.Default;
        }

        /// <inheritdoc/>
        public async IAsyncEnumerable<TResult> ExecuteAsyncRemoteStream<TResult>(Expression expression, [EnumeratorCancellation] CancellationToken cancellation)
        {
            ExpressionHelper.CheckExpressionResultType<TResult>(expression);

            cancellation.ThrowIfCancellationRequested();
            var rlinq = _context.ExpressionTranslator.TranslateExpression(expression);

            cancellation.ThrowIfCancellationRequested();
            var asyncEnumerable = _dataProvider(rlinq, cancellation);

            cancellation.ThrowIfCancellationRequested();
            await foreach (var resultItem in asyncEnumerable.WithCancellation(cancellation))
            {
                cancellation.ThrowIfCancellationRequested();
                var result = await _resultMapper.MapResultAsync<TResult>(resultItem, expression, cancellation).ConfigureAwait(false);
                yield return result;
            }
        }

        /// <inheritdoc/>
        IQueryable IQueryProvider.CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.CheckNotNull(nameof(expression)).Type)
                ?? throw new RemoteLinqException($"Failed to get element type of {expression.Type}");
            return new AsyncRemoteStreamQueryable(elementType, this, expression);
        }

        /// <inheritdoc/>
        IQueryable<TElement> IQueryProvider.CreateQuery<TElement>(Expression expression)
            => new AsyncRemoteStreamQueryable<TElement>(this, expression);

        /// <summary>
        /// This operation must not be used on stream queryable.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown for stream queryable.</exception>
        object IQueryProvider.Execute(Expression expression)
            => throw AsyncRemoteStreamQueryable.QueryOperationNotSupportedException;

        /// <summary>
        /// This operation must not be used on stream queryable.
        /// </summary>
        /// <exception cref="NotSupportedException">Always thrown for stream queryable.</exception>
        TResult IQueryProvider.Execute<TResult>(Expression expression)
            => throw AsyncRemoteStreamQueryable.QueryOperationNotSupportedException;
    }
}