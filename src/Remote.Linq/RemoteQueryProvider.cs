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
        private static readonly System.Reflection.MethodInfo MapResultMethodInfo = typeof(RemoteQueryProvider).GetMethod("MapResult", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
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
            var exp = expression.ReplaceAnonymousTypes();

            var rlinq1 = exp.ToRemoteLinqExpression();
            var rlinq2 = rlinq1.ReplaceQueryableByResourceDescriptors();

            var dataRecords = _dataProvider(rlinq2);

            var result = MapResult<TResult>(dataRecords);
            return result;
        }

        private static TResult MapResult<TResult>(IEnumerable<DynamicObject> dataRecords)
        {
            if (typeof(TResult).IsAssignableFrom(typeof(IEnumerable<DynamicObject>)))
            {
                return (TResult)dataRecords;
            }

            var elementType = TypeHelper.GetElementType(typeof(TResult));

            if (dataRecords.Any())
            {
                if (dataRecords.All(item => item.Count == 1 && string.IsNullOrEmpty(item.Keys.Single())))
                {
                    // project single property
                    var items = dataRecords.SelectMany(i => i.Values).ToArray();
                    var r1 = MethodInfos.Enumerable.Cast.MakeGenericMethod(elementType).Invoke(null, new[] { items });
                    var r2 = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).Invoke(null, new[] { r1 });
                    return (TResult)r2;
                }
                else
                {
                    // project data record
                    var propertyCount = dataRecords.First().Count;
                    var propertyTypes = new Type[propertyCount];
                    for (int i = 0; i < propertyCount; i++)
                    {
                        var value = dataRecords.Select(record => record.Values.ElementAt(i)).Where(x => !ReferenceEquals(null, x)).FirstOrDefault();
                        propertyTypes[i] = ReferenceEquals(null, value) ? null : value.GetType();
                    }

                    var constructors = elementType.GetConstructors()
                        .Select(i => new { Info = i, Parameters = i.GetParameters() })
                        .OrderByDescending(i => i.Parameters.Length).ToList();
                    var constructor = constructors.FirstOrDefault(ctor =>
                    {
                        if (ctor.Parameters.Length != propertyCount) return false;
                        for (int i = 0; i < propertyCount; i++)
                        {
                            var propertyType = propertyTypes[i];
                            if (propertyType == null || propertyType == typeof(DynamicObject)) continue;
                            var parameterType = ctor.Parameters[i];
                            if (!parameterType.ParameterType.IsAssignableFrom(propertyType)) return false;
                        }
                        return true;
                    });

                    if (constructor == null)
                    {
                        constructor = constructors.SingleOrDefault(x => x.Parameters.Length == 0);
                    }

                    if (constructor != null)
                    {
                        var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                        foreach (var item in dataRecords)
                        {
                            var values = item.Values.Select((x, i) =>
                            {
                                if (x is DynamicObject)
                                {
                                    // subsequent mapping of nested anonymous type
                                    var targetType = constructor.Parameters[i].ParameterType;
                                    var targetTypeEnumerable = typeof(IEnumerable<>).MakeGenericType(targetType);
                                    var method = MapResultMethodInfo.MakeGenericMethod(targetTypeEnumerable);
                                    var args = new[] { (DynamicObject)x }.AsEnumerable();
                                    //var mappedValues = (IEnumerable<object>)method.Invoke(null, new[] { args });
                                    var mappedValues = (System.Collections.IEnumerable)method.Invoke(null, new[] { args });
                                    var mappedValue = mappedValues.Cast<object>().Single();
                                    return mappedValue;
                                }
                                else
                                {
                                    return x;
                                }
                            });
                            var dataRecord = constructor.Info.Invoke(values.ToArray());
                            list.Add(dataRecord);
                        }
                        return (TResult)list;
                    }
                }
            }

            throw new Exception(string.Format("Failed to project result into type {0}", typeof(TResult).FullName));
        }

        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
    }
}
