// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Remote.Linq
{
    internal sealed partial class RemoteQueryProvider : IQueryProvider
    {
        private readonly Func<Expressions.Expression, IEnumerable<DynamicObject>> _dataProvider;
        private readonly IDynamicObjectMapper _mapper;
        private readonly ITypeResolver _typeResolver;

        internal RemoteQueryProvider(Func<Expressions.Expression, IEnumerable<DynamicObject>> dataProvider, ITypeResolver typeResolver, IDynamicObjectMapper mapper)
        {
            if (ReferenceEquals(null, dataProvider)) throw new ArgumentNullException("dataProvider");
            _dataProvider = dataProvider;
            _mapper = mapper;
            _typeResolver = typeResolver;
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
            var rlinq = TranslateExpression(expression, _typeResolver);
            var dataRecords = _dataProvider(rlinq);
            return MapToType<TResult>(dataRecords, _mapper);
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }

        internal static Expressions.Expression TranslateExpression(Expression expression, ITypeResolver typeResolver)
        {
            var rlinq1 = expression.ToRemoteLinqExpression();
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors(typeResolver);
            return rlinq2;
        }

        internal static T MapToType<T>(IEnumerable<DynamicObject> dataRecords, IDynamicObjectMapper mapper)
        {
            var elementType = TypeHelper.GetElementType(typeof(T));

            if (ReferenceEquals(null, mapper))
            {
                mapper = new DynamicObjectMapper();
            }

            var result = mapper.Map(dataRecords, elementType);

            if (ReferenceEquals(null, result))
            {
                return default(T);
            }

            if (typeof(T).IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(elementType)))
            {
                return (T)result;
            }

            if (typeof(T).IsAssignableFrom(elementType))
            {
                try
                {
                    var single = MethodInfos.Enumerable.Single.MakeGenericMethod(elementType).Invoke(null, new object[] { result });
                    return (T)single;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }

            throw new Exception(string.Format("Failed to cast result of type '{0}' to '{1}'", result.GetType(), typeof(T)));
        }
    }
}