// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Remote.Linq
{
    internal sealed partial class AsyncRemoteQueryProvider : IAsyncQueryProvider
    {
        private readonly Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> _dataProvider;
        private readonly ITypeResolver _typeResolver;
        private readonly Func<IDynamicObjectMapper> _mapper;

        internal AsyncRemoteQueryProvider(Func<Expressions.Expression, Task<IEnumerable<DynamicObject>>> dataProvider, ITypeResolver typeResolver, Func<IDynamicObjectMapper> mapper)
        {
            if (ReferenceEquals(null, dataProvider)) throw new ArgumentNullException("dataProvider");
            _dataProvider = dataProvider;
            _typeResolver = typeResolver;
            _mapper = mapper;
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
            var rlinq = RemoteQueryProvider.TranslateExpression(expression, _typeResolver);
            var task = _dataProvider(rlinq);
            IEnumerable<DynamicObject> dataRecords;
            try
            {
                dataRecords = task.Result;
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
            return RemoteQueryProvider.MapToType<TResult>(dataRecords, _mapper);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        public async Task<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var rlinq = RemoteQueryProvider.TranslateExpression(expression, _typeResolver);
            var dataRecords = await _dataProvider(rlinq);
            return RemoteQueryProvider.MapToType<TResult>(dataRecords, _mapper);
        }
    }
}
