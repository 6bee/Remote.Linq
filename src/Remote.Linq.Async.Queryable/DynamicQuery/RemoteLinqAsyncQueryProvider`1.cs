// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Async.Queryable.DynamicQuery
{
    using Aqua.TypeExtensions;
    using Aqua.TypeSystem;
    using Remote.Linq.DynamicQuery;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Security;
    using System.Threading;
    using System.Threading.Tasks;
    using MethodInfo = System.Reflection.MethodInfo;
    using RemoteLinq = Remote.Linq.Expressions;

    internal sealed class RemoteLinqAsyncQueryProvider<TSource> : IRemoteLinqAsyncQueryProvider
    {
        private readonly Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> _asyncDataProvider;
        private readonly ITypeInfoProvider? _typeInfoProvider;
        private readonly IQueryResultMapper<TSource> _resultMapper;
        private readonly Func<Expression, bool>? _canBeEvaluatedLocally;

        [SecuritySafeCritical]
        public RemoteLinqAsyncQueryProvider(Func<RemoteLinq.Expression, CancellationToken, IAsyncEnumerable<TSource>> asyncDataProvider, ITypeInfoProvider? typeInfoProvider, Func<Expression, bool>? canBeEvaluatedLocally, IQueryResultMapper<TSource> resultMapper)
        {
            _asyncDataProvider = asyncDataProvider.CheckNotNull(nameof(asyncDataProvider));
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _typeInfoProvider = typeInfoProvider;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        public IAsyncQueryable<TElement> CreateQuery<TElement>(Expression expression)
             => new RemoteLinqAsyncQueryable<TElement>(this, expression);

        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellation)
        {
            if (typeof(TResult).Implements(typeof(IAsyncEnumerable<>), out var genericArguments))
            {
                ExpressionHelper.CheckExpressionResultType<TResult>(expression);

                cancellation.ThrowIfCancellationRequested();
                var rlinq = ExpressionHelper.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

                cancellation.ThrowIfCancellationRequested();
                var asyncEnumerable = _asyncDataProvider(rlinq, cancellation);

                var mappedEnumerable = CallMapAsyncEnumerable<TResult>(genericArguments[0], asyncEnumerable, cancellation);

                return new ValueTask<TResult>(mappedEnumerable);
            }

            ExpressionHelper.CheckExpressionResultType<ValueTask<TResult>>(expression);

            throw new NotSupportedException($"Async remote stream may only be executed as IAsyncEnumerable<>. Consider calling AsAsyncEnumerable() extension method before enumerating.");
        }

        private static readonly MethodInfo _asyncEnumerableMethodInfo = typeof(RemoteLinqAsyncQueryProvider<TSource>).GetMethod(nameof(MapAsyncEnumerable), BindingFlags.NonPublic | BindingFlags.Instance);

        private TResult CallMapAsyncEnumerable<TResult>(Type elementType, IAsyncEnumerable<TSource> source, CancellationToken cancellation)
        {
            var method = _asyncEnumerableMethodInfo.MakeGenericMethod(elementType);
            var result = method.Invoke(this, new object[] { source, cancellation });
            return (TResult)result;
        }

        private async IAsyncEnumerable<T> MapAsyncEnumerable<T>(IAsyncEnumerable<TSource> source, [EnumeratorCancellation] CancellationToken cancellation)
        {
            await foreach (var item in source.WithCancellation(cancellation).ConfigureAwait(false))
            {
                yield return _resultMapper.MapResult<T>(item, null!);
            }
        }
    }
}
