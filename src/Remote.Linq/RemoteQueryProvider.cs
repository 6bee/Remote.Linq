// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Remote.Linq
{
    internal sealed class RemoteQueryProvider : IQueryProvider
    {
        private readonly Func<Expressions.Expression, IEnumerable<DynamicObject>> _dataProvider;

        public RemoteQueryProvider(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider)
        {
            if (dataProvider == null) throw new ArgumentNullException("dataProvider");
            _dataProvider = dataProvider;
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new RemoteQueryable<TElement>(this, expression);
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var elementType = TypeHelper.GetElementType(expression.Type);
            return new RemoteQueryable(elementType, this, expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var rlinq1 = expression.ToRemoteLinqExpression();
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors();

            var dataRecords = _dataProvider(rlinq2);

            var elementType = TypeHelper.GetElementType(typeof(TResult));
            var mapper = DynamicObjectMapper.MapDynamicObjectListMethod.MakeGenericMethod(elementType);
            var result = mapper.Invoke(null, new object[] { dataRecords });

            if (ReferenceEquals(null, result))
            {
                return default(TResult);
            }
            
            if (typeof(TResult).IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(elementType)))
            {
                return (TResult)result;
            }
            
            if (typeof(TResult).IsAssignableFrom(elementType))
            {
                try
                {
                    var single = MethodInfos.Enumerable.Single.MakeGenericMethod(elementType).Invoke(null, new object[] { result });
                    return (TResult)single;
                }
                catch (System.Reflection.TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }
            
            throw new Exception(string.Format("Failed to cast result of type '{0}' to '{1}'", result.GetType(), typeof(TResult)));
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
