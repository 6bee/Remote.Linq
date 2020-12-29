// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Expression = System.Linq.Expressions.Expression;
    using MethodInfo = System.Reflection.MethodInfo;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    internal sealed class AsyncRemoteQueryProvider<TSource> : IAsyncRemoteQueryProvider
    {
        private static readonly MethodInfo _executeMethod = typeof(AsyncRemoteQueryProvider<TSource>)
            .GetMethods()
            .Single(x => x.IsGenericMethod && string.Equals(x.Name, nameof(Execute), StringComparison.Ordinal));

        private readonly Func<Expressions.Expression, CancellationToken, Task<TSource>> _asyncDataProvider;
        private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
        private readonly ITypeInfoProvider? _typeInfoProvider;
        private readonly Func<Expression, bool>? _canBeEvaluatedLocally;

        internal AsyncRemoteQueryProvider(Func<Expressions.Expression, CancellationToken, Task<TSource>> asyncDataProvider, ITypeInfoProvider? typeInfoProvider, IAsyncQueryResultMapper<TSource> resultMapper, Func<Expression, bool>? canBeEvaluatedLocally)
        {
            _asyncDataProvider = asyncDataProvider.CheckNotNull(nameof(asyncDataProvider));
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _typeInfoProvider = typeInfoProvider;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) => new AsyncRemoteQueryable<TElement>(this, expression);

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.CheckNotNull(nameof(expression)).Type)
                ?? throw new RemoteLinqException($"Failed to get element type of {expression.Type}");
            return new RemoteQueryable(elementType, this, expression);
        }

        [return: MaybeNull]
        public TResult Execute<TResult>(Expression expression)
        {
            ExpressionHelper.CheckExpressionResultType<TResult>(expression);

            var rlinq = ExpressionHelper.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

            var task = _asyncDataProvider(rlinq, CancellationToken.None);

            TResult result;
            try
            {
                var dataRecords = task.Result;

                if (Equals(default(TSource), dataRecords))
                {
                    result = default;
                }
                else
                {
                    var mappingTask = _resultMapper.MapResultAsync<TResult>(dataRecords, expression, CancellationToken.None);
                    result = mappingTask.Result;
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count > 0)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

#nullable disable
        public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellation)
        {
            ExpressionHelper.CheckExpressionResultType<TResult>(expression);

            var rlinq = ExpressionHelper.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

            var dataRecords = await _asyncDataProvider(rlinq, cancellation).ConfigureAwait(false);

            var result = await _resultMapper.MapResultAsync<TResult>(dataRecords, expression, cancellation).ConfigureAwait(false);

            return result;
        }
#nullable restore

        public object? Execute(Expression expression) => this.InvokeAndUnwrap<object?>(_executeMethod, expression);
    }
}