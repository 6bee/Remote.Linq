// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TestSupport
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Expression = System.Linq.Expressions.Expression;

    /// <summary>
    /// <see cref="TaskAsyncQueryProvider"/> mimics expression execution as like in a client-server-round-trip using remote linq,
    /// allowing to wrap local test data in a <see cref="IAsyncRemoteQueryable{T}"/> type.
    /// </summary>
    internal sealed class TaskAsyncQueryProvider : IAsyncRemoteQueryProvider
    {
        public IQueryable CreateQuery(Expression expression)
            => CreateQuery<object>(expression);

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncRemoteQueryable<TElement>(this, expression);

        public object Execute(Expression expression)
            => Execute<object>(expression);

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
            => Task.FromResult(Execute<TResult>(expression));

        public TResult Execute<TResult>(Expression expression)
        {
            var systemlinq = expression.SimplifyIncorporationOfRemoteQueryables();
            var remotelinq = systemlinq
                .ToRemoteLinqExpression()
                .ReplaceGenericQueryArgumentsByNonGenericArguments();
            var queryresult = remotelinq.Execute(null);
            var result = DynamicResultMapper.MapToType<TResult>(queryresult, null, expression);
            return result;
        }
    }
}