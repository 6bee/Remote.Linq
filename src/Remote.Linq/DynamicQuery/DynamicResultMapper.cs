// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.DynamicQuery
{
    using Aqua.Dynamic;
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;

    public sealed class DynamicResultMapper : IQueryResultMapper<DynamicObject>
    {
        private readonly IDynamicObjectMapper? _mapper;

        public DynamicResultMapper(IDynamicObjectMapper? mapper)
        {
            _mapper = mapper;
        }

        public TResult? MapResult<TResult>(DynamicObject? source, Expression expression)
            => MapToType<TResult>(source, _mapper, expression);

        [return: MaybeNull]
        internal static TResult MapToType<TResult>(DynamicObject? dataRecords, IDynamicObjectMapper? mapper, Expression expression)
        {
            if (dataRecords is null)
            {
                return default;
            }

            mapper ??= new DynamicQueryResultMapper().ValueMapper;

            var elementType = TypeHelper.GetElementType(typeof(TResult));

            var result = GetResultFromDynamicObject<TResult>(dataRecords, elementType, mapper);

            if (result is null)
            {
                return default;
            }

            if (result is TResult r)
            {
                return r;
            }

            if (typeof(TResult).IsAssignableFrom(typeof(IEnumerable<>).MakeGenericType(elementType)))
            {
                return (TResult)result;
            }

            if (result is System.Collections.IEnumerable enumerable && typeof(TResult).IsAssignableFrom(elementType) && expression is MethodCallExpression methodCallExpression)
            {
                return MapToSingleResult<TResult>(elementType, enumerable, methodCallExpression);
            }

            throw new RemoteLinqException($"Failed to cast result of type '{result.GetType()}' to '{typeof(TResult)}'");
        }

        private static object? GetResultFromDynamicObject<TResult>(DynamicObject dataRecords, Type elementType, IDynamicObjectMapper mapper)
        {
            if (elementType == typeof(TResult))
            {
                // handle special case of single item query from too small or too large result set.
                var properties = dataRecords.Properties;
                if (properties?.Count == 1)
                {
                    var p = properties.Single();
                    if (string.IsNullOrEmpty(p.Name) && p.Value is object[] objectArray)
                    {
                        if (objectArray.Length == 0)
                        {
                            return Enumerable.Empty<TResult>();
                        }

                        if (objectArray.All(x => x is null || (x is DynamicObject dyn && dyn.IsNull)))
                        {
                            return new TResult[objectArray.Length];
                        }
                    }
                }
            }

            return mapper.Map(dataRecords, typeof(TResult));
        }

        [return: MaybeNull]
        internal static TResult MapToSingleResult<TResult>(Type elementType, System.Collections.IEnumerable result, MethodCallExpression methodCallExpression)
        {
            var hasPredicate = methodCallExpression.Arguments.Count == 2;
            var arguments = hasPredicate
                ? new object[] { result, GetTruePredicate(elementType) }
                : new object[] { result };
            var method = methodCallExpression.Method.Name.EndsWith("OrDefault", StringComparison.Ordinal)
                ? (hasPredicate ? MethodInfos.Enumerable.SingleOrDefaultWithPredicate : MethodInfos.Enumerable.SingleOrDefault)
                : (hasPredicate ? MethodInfos.Enumerable.SingleWithPredicate : MethodInfos.Enumerable.Single);
            var single = method.MakeGenericMethod(elementType).InvokeAndUnwrap(null, arguments);
            return (TResult)single!;
        }

        private static object GetTruePredicate(Type t)
            => Expression.Lambda(Expression.Constant(true), Expression.Parameter(t)).Compile();
    }
}