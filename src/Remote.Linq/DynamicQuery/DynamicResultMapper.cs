// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;

    internal sealed class DynamicResultMapper : IQueryResultMapper<IEnumerable<DynamicObject>>
    {
        private readonly IDynamicObjectMapper _mapper;

        public DynamicResultMapper(IDynamicObjectMapper mapper)
        {
            _mapper = mapper;
        }

        public TResult MapResult<TResult>(IEnumerable<DynamicObject> source, Expression expression)
            => MapToType<TResult>(source, _mapper, expression);

        internal static T MapToType<T>(IEnumerable<DynamicObject> dataRecords, IDynamicObjectMapper mapper, Expression expression)
        {
            var elementType = TypeHelper.GetElementType(typeof(T));

            if (mapper is null)
            {
                mapper = new DynamicQueryResultMapper();
            }

            if (dataRecords == null)
            {
                return default;
            }

            var result = mapper.Map(dataRecords, elementType);

            if (result is null)
            {
                return default;
            }

            if (result is T || typeof(T).IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(elementType)))
            {
                return (T)result;
            }

            if (typeof(T).IsAssignableFrom(elementType))
            {
                try
                {
                    object single;
                    if ((expression as MethodCallExpression)?.Arguments.Count == 2)
                    {
                        var predicate = GetTruePredicate(elementType);
                        single = MethodInfos.Enumerable.SingleWithFilter.MakeGenericMethod(elementType).Invoke(null, new object[] { result, predicate });
                    }
                    else
                    {
                        single = MethodInfos.Enumerable.Single.MakeGenericMethod(elementType).Invoke(null, new object[] { result });
                    }

                    return (T)single;
                }
                catch (TargetInvocationException ex)
                {
                    throw ex.InnerException;
                }
            }

            throw new Exception($"Failed to cast result of type '{result.GetType()}' to '{typeof(T)}'");
        }

        private static object GetTruePredicate(Type t)
            => Expression.Lambda(Expression.Constant(true), Expression.Parameter(t)).Compile();
    }
}
