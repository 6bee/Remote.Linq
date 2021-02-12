// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using MethodInfo = System.Reflection.MethodInfo;
    using RemoteLinq = Remote.Linq.Expressions;
    using SystemLinq = System.Linq.Expressions;

    [SuppressMessage("Minor Code Smell", "S4136:Method overloads should be grouped together", Justification = "Methods appear in logical order")]
    internal sealed class AsyncRemoteQueryProvider<TSource> : IAsyncRemoteQueryProvider
    {
        private static readonly MethodInfo _executeMethod = typeof(AsyncRemoteQueryProvider<TSource>)
            .GetMethods()
            .Single(x => x.IsGenericMethod && string.Equals(x.Name, nameof(Execute), StringComparison.Ordinal));

        private readonly Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> _asyncDataProvider;
        private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
        private readonly ITypeInfoProvider? _typeInfoProvider;
        private readonly Func<SystemLinq.Expression, bool>? _canBeEvaluatedLocally;

        internal AsyncRemoteQueryProvider(Func<RemoteLinq.Expression, CancellationToken, ValueTask<TSource?>> asyncDataProvider, ITypeInfoProvider? typeInfoProvider, IAsyncQueryResultMapper<TSource> resultMapper, Func<SystemLinq.Expression, bool>? canBeEvaluatedLocally)
        {
            _asyncDataProvider = asyncDataProvider.CheckNotNull(nameof(asyncDataProvider));
            _resultMapper = resultMapper.CheckNotNull(nameof(resultMapper));
            _typeInfoProvider = typeInfoProvider;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        public IQueryable<TElement> CreateQuery<TElement>(SystemLinq.Expression expression) => new AsyncRemoteQueryable<TElement>(this, expression);

        public IQueryable CreateQuery(SystemLinq.Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.CheckNotNull(nameof(expression)).Type)
                ?? throw new RemoteLinqException($"Failed to get element type of {expression.Type}");
            return new RemoteQueryable(elementType, this, expression);
        }

        [return: MaybeNull]
        public TResult Execute<TResult>(SystemLinq.Expression expression)
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
                    var mappingTask = _resultMapper.MapResultAsync<TResult>(dataRecords, expression, CancellationToken.None).AsTask();
                    result = mappingTask.Result;
                }
            }
            catch (AggregateException ex)
            {
                if (ex.InnerException is not null)
                {
                    throw ex.InnerException!;
                }
                else
                {
                    throw;
                }
            }

            return result;
        }

        public async ValueTask<TResult?> ExecuteAsync<TResult>(SystemLinq.Expression expression, CancellationToken cancellation)
        {
            ExpressionHelper.CheckExpressionResultType<TResult>(expression);

            var rlinq = ExpressionHelper.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

            var dataRecords = await _asyncDataProvider(rlinq, cancellation).ConfigureAwait(false);

            var result = await _resultMapper.MapResultAsync<TResult>(dataRecords, expression, cancellation).ConfigureAwait(false);

            return result;
        }

        public object? Execute(SystemLinq.Expression expression) => this.InvokeAndUnwrap<object?>(_executeMethod, expression);
    }
}