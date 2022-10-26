// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TestSupport
{
    using Aqua.TypeExtensions;
    using Remote.Linq.DynamicQuery;
    using Remote.Linq.ExpressionExecution;
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
            .GetMethodEx(nameof(Execute), new[] { typeof(MethodInfos.TResult) }, typeof(Expression));

        private static readonly MethodInfo _createQueryMethod = typeof(TaskAsyncQueryProvider)
            .GetMethodEx(nameof(CreateQuery), new[] { typeof(MethodInfos.TElement) }, typeof(Expression));

        private readonly IExpressionTranslatorContext _context;

        public TaskAsyncQueryProvider(IExpressionTranslatorContext? context = null)
            => _context = context ?? ExpressionTranslatorContext.Default;

        /// <inheritdoc/>
        public IQueryable CreateQuery(Expression expression)
            => this.InvokeAndUnwrap<IQueryable>(_createQueryMethod, expression);

        /// <inheritdoc/>
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncRemoteQueryable<TElement>(this, expression);

        /// <inheritdoc/>
        public ValueTask<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellation)
            => new(Execute<TResult>(expression)!);

        /// <inheritdoc/>
        public object? Execute(Expression expression)
            => this.InvokeAndUnwrap<object?>(_executeMethod, expression);

        /// <inheritdoc/>
        public TResult Execute<TResult>(Expression expression)
        {
            // TODO: verify modificatiopn does no harm as it's adding "exp.ReplaceQueryableByResourceDescriptors(_context?.TypeInfoProvider)"
            var remotelinq = _context.ExpressionTranslator.TranslateExpression(expression);
            var queryresult = remotelinq.Execute(null!, _context);
            var result = DynamicResultMapper.MapToType<TResult>(queryresult, null, expression);
            return result!;
        }
    }
}