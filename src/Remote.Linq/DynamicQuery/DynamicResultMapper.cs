// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.Dynamic;
using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Remote.Linq.DynamicQuery
{
    internal sealed class DynamicResultMapper : IQueryResultMapper<IEnumerable<DynamicObject>>
    {
        private readonly IDynamicObjectMapper _mapper;

        public DynamicResultMapper(IDynamicObjectMapper mapper)
        {
            _mapper = mapper;
        }

        public TResult MapResult<TResult>(IEnumerable<DynamicObject> source)
        {
            return MapToType<TResult>(source, _mapper);
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
