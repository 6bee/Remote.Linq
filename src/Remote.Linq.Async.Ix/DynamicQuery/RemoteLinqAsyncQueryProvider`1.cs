// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Ix.DynamicQuery
{
    using Aqua.TypeSystem;
    using Aqua.TypeSystem.Extensions;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using RemoteLinq = Remote.Linq.Expressions;

    internal sealed class RemoteLinqAsyncQueryProvider<TSource> : IRemoteLinqAsyncQueryProvider
    {
        private readonly Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> _dataProvider;
        private readonly ITypeInfoProvider? _typeInfoProvider;
        private readonly IQueryResultMapper<TSource> _resultMapper;
        private readonly Func<Expression, bool>? _canBeEvaluatedLocally;

        [SecuritySafeCritical]
        public RemoteLinqAsyncQueryProvider(Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> dataProvider, ITypeInfoProvider? typeInfoProvider, Func<Expression, bool>? canBeEvaluatedLocally, IQueryResultMapper<TSource> resultMapper)
        {
            _dataProvider = dataProvider.CheckNotNull(nameof(dataProvider));
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _typeInfoProvider = typeInfoProvider;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
             => new RemoteLinqAsyncQueryable<TElement>(this, expression);

        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken token)
        {
            var isAsyncEnumerable = typeof(TResult).Implements(typeof(IAsyncEnumerable<>), out var genericArguments);
            if (isAsyncEnumerable)
            {
                ExpressionHelper.CheckExpressionResultType<TResult>(expression);
            }
            else
            {
                ExpressionHelper.CheckExpressionResultType<ValueTask<TResult>>(expression);
            }

            token.ThrowIfCancellationRequested();
            var rlinq = ExpressionHelper.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

            token.ThrowIfCancellationRequested();
            var enumerable = _dataProvider(rlinq, token);

            token.ThrowIfCancellationRequested();

            if (isAsyncEnumerable)
            {
                var r = (TResult)enumerable;
                return new ValueTask<TResult>(r);
            }

            ////return await new ValueTask<TResult>(default(TResult)).ConfigureAwait(false);

            return default;
            ////return new Core();

            ////async Task<TResult> GetAsyncEnumerable()
            ////{

            ////};

            ////await foreach (var resultItem in asyncEnumerable.WithCancellation(token))
            ////{
            ////    token.ThrowIfCancellationRequested();
            ////    var result = _resultMapper.MapResult<TResult>(resultItem, expression);
            ////    yield return result;
            ////}
        }

        public async IAsyncEnumerator<T> GetAsyncEnumerator<T>(Expression expression, CancellationToken token)
        {
            ExpressionHelper.CheckExpressionResultType<IAsyncEnumerable<T>>(expression);

            token.ThrowIfCancellationRequested();
            var rlinq = ExpressionHelper.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

            token.ThrowIfCancellationRequested();
            var enumerable = _dataProvider(rlinq, token);

            await foreach (var item in enumerable.WithCancellation(token))
            {
                var mappedItem = _resultMapper.MapResult<T>(item, null!);
                yield return mappedItem;
            }
        }
    }
}
