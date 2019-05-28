// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;

    internal sealed partial class AsyncRemoteQueryProvider<TSource> : IAsyncRemoteQueryProvider
    {
        private readonly Func<Expressions.Expression, CancellationToken, Task<TSource>> _asyncDataProvider;
        private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
        private readonly ITypeInfoProvider _typeInfoProvider;
        private readonly Func<Expression, bool> _canBeEvaluatedLocally;

        internal AsyncRemoteQueryProvider(Func<Expressions.Expression, CancellationToken, Task<TSource>> asyncDataProvider, ITypeInfoProvider typeInfoProvider, IAsyncQueryResultMapper<TSource> resutMapper, Func<Expression, bool> canBeEvaluatedLocally)
        {
            _asyncDataProvider = asyncDataProvider ?? throw new ArgumentNullException(nameof(asyncDataProvider));
            _resultMapper = resutMapper;
            _typeInfoProvider = typeInfoProvider;
            _canBeEvaluatedLocally = canBeEvaluatedLocally;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
            => new AsyncRemoteQueryable<TElement>(this, expression);

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type);
            return new RemoteQueryable(elementType, this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rlinq = RemoteQueryProvider<TSource>.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

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

        public object Execute(Expression expression)
            => throw new NotImplementedException();

        public async Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var rlinq = RemoteQueryProvider<TSource>.TranslateExpression(expression, _typeInfoProvider, _canBeEvaluatedLocally);

            var dataRecords = await _asyncDataProvider(rlinq, cancellationToken);

            var result = await _resultMapper.MapResultAsync<TResult>(dataRecords, expression, cancellationToken);

            return result;
        }
    }
}
