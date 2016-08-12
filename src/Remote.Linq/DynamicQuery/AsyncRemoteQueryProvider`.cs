// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.TypeSystem;
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;

    internal sealed partial class AsyncRemoteQueryProvider<TSource> : IAsyncRemoteQueryProvider
    {
        private readonly Func<Expressions.Expression, Task<TSource>> _dataProvider;
        private readonly IAsyncQueryResultMapper<TSource> _resultMapper;
        private readonly ITypeResolver _typeResolver;

        internal AsyncRemoteQueryProvider(Func<Expressions.Expression, Task<TSource>> dataProvider, ITypeResolver typeResolver, IAsyncQueryResultMapper<TSource> resutMapper)
        {
            if (ReferenceEquals(null, dataProvider))
            {
                throw new ArgumentNullException(nameof(dataProvider));
            }

            _dataProvider = dataProvider;
            _resultMapper = resutMapper;
            _typeResolver = typeResolver;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new AsyncRemoteQueryable<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type);
            return new RemoteQueryable(elementType, this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rlinq = RemoteQueryProvider<TSource>.TranslateExpression(expression, _typeResolver);

            var task = _dataProvider(rlinq);

            TResult result;
            try
            {
                var dataRecords = task.Result;

                var mappingTask = _resultMapper.MapResultAsync<TResult>(dataRecords);

                result = mappingTask.Result;
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
        {
            throw new NotImplementedException();
        }
        public async Task<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var rlinq = RemoteQueryProvider<TSource>.TranslateExpression(expression, _typeResolver);

            var dataRecords = await _dataProvider(rlinq);
            
            var result = await _resultMapper.MapResultAsync<TResult>(dataRecords);

            return result;
        }
    }
}
