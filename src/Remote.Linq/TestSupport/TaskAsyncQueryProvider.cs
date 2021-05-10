﻿// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TestSupport
{
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.Expressions;
    using Remote.Linq.ExpressionVisitors;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Expression = System.Linq.Expressions.Expression;
    using MethodInfo = System.Reflection.MethodInfo;

    /// <summary>
    /// <see cref="TaskAsyncQueryProvider"/> mimics asynchronous expression execution as in a client-server-round-trip using remote linq,
    /// allowing to wrap local test data in a <see cref="IAsyncRemoteQueryable{T}"/> type.
    /// </summary>
    internal sealed class TaskAsyncQueryProvider : IAsyncRemoteQueryProvider
    {
        private static readonly MethodInfo _executeMethod = typeof(TaskAsyncQueryProvider)
            .GetMethods()
            .Single(x => x.IsGenericMethod && string.Equals(x.Name, nameof(Execute), StringComparison.Ordinal));

        private static readonly MethodInfo _createQueryMethod = typeof(TaskAsyncQueryProvider)
            .GetMethods()
            .Single(x => x.IsGenericMethod && string.Equals(x.Name, nameof(CreateQuery), StringComparison.Ordinal));

        private readonly IExpressionTranslatorContext? _context;

        public TaskAsyncQueryProvider(IExpressionTranslatorContext? context = null)
            => _context = context;

        public IQueryable CreateQuery(Expression expression)
            => this.InvokeAndUnwrap<IQueryable>(_createQueryMethod, expression) !;

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncRemoteQueryable<TElement>(this, expression);

        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellation)
            => new ValueTask<TResult>(Execute<TResult>(expression) !);

        public object? Execute(Expression expression)
            => this.InvokeAndUnwrap<object>(_executeMethod, expression);

        public TResult Execute<TResult>(Expression expression)
        {
            var systemlinq = expression.SimplifyIncorporationOfRemoteQueryables();
            var remotelinq = systemlinq
                .ToRemoteLinqExpression(_context)
                .ReplaceGenericQueryArgumentsByNonGenericArguments();
            var queryresult = remotelinq.Execute(null!, _context);
            var result = DynamicResultMapper.MapToType<TResult>(queryresult, null, expression);
            return result!;
        }
    }
}