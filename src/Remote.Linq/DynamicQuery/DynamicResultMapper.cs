// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal sealed class DynamicResultMapper : IQueryResultMapper<IEnumerable<DynamicObject>>
    {
        private readonly IDynamicObjectMapper _mapper;

        public DynamicResultMapper(IDynamicObjectMapper mapper)
        {
            _mapper = mapper;
        }

        public TResult MapResult<TResult>(IEnumerable<DynamicObject> source, Expression expression)
            => MapToType<TResult>(source, _mapper, expression);

        internal static TResult MapToType<TResult>(IEnumerable<DynamicObject> dataRecords, IDynamicObjectMapper mapper, Expression expression)
        {
            var elementType = TypeHelper.GetElementType(typeof(TResult));

            if (mapper is null)
            {
                mapper = new DynamicQueryResultMapper();
            }

            if (dataRecords is null)
            {
                return default;
            }

            var result = mapper.Map(dataRecords, elementType);

            if (result is null)
            {
                return default;
            }

            if (result is TResult || typeof(TResult).IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(elementType)))
            {
                return (TResult)result;
            }

            if (typeof(TResult).IsAssignableFrom(elementType) && expression is MethodCallExpression methodCallExpression)
            {
                return MapToSingleResult<TResult>(elementType, result, methodCallExpression);
            }

            throw new Exception($"Failed to cast result of type '{result.GetType()}' to '{typeof(TResult)}'");
        }

        internal static TResult MapToSingleResult<TResult>(Type elementType, System.Collections.IEnumerable result, MethodCallExpression methodCallExpression)
        {
            var hasPredicate = methodCallExpression.Arguments.Count == 2;
            var arguments = hasPredicate
                ? new object[] { result, GetTruePredicate(elementType) }
                : new object[] { result };
            var method = methodCallExpression.Method.Name.EndsWith("OrDefault")
                ? (hasPredicate ? MethodInfos.Enumerable.SingleOrDefaultWithPredicate : MethodInfos.Enumerable.SingleOrDefault)
                : (hasPredicate ? MethodInfos.Enumerable.SingleWithPredicate : MethodInfos.Enumerable.Single);
            object single = method.MakeGenericMethod(elementType).InvokeAndUnwrap(null, arguments);
            return (TResult)single;
        }

        private static object GetTruePredicate(Type t)
            => Expression.Lambda(Expression.Constant(true), Expression.Parameter(t)).Compile();
    }
}
