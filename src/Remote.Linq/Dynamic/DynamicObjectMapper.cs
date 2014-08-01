// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Remote.Linq.Dynamic
{
    public static class DynamicObjectMapper
    {
        internal static readonly System.Reflection.MethodInfo MapDynamicObjectListMethod = typeof(DynamicObjectMapper)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "Map")
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Where(x => x.GetParameters().Length == 1 && x.GetParameters()[0].ParameterType == typeof(IEnumerable<DynamicObject>))
            .Single();

        public static T Map<T>(DynamicObject obj)
        {
            return Map<T>(new[] { obj }).SingleOrDefault();
        }

        public static IEnumerable<T> Map<T>(IEnumerable<DynamicObject> objects)
        {
            if (ReferenceEquals(null, objects))
            {
                return null;
            }

            if (!objects.Any())
            {
                return new T[0];
            }

            if (typeof(T).IsAssignableFrom(typeof(DynamicObject)))
            {
                return objects.Cast<T>();
            }

            var elementType = TypeHelper.GetElementType(typeof(T));
            if (objects.Any())
            {
                if (objects.All(item => item.Count == 1 && string.IsNullOrEmpty(item.Keys.Single())))
                {
                    // project single property
                    var items = objects.SelectMany(i => i.Values).Where(x => !ReferenceEquals(null, x)).ToArray();
                    var r1 = MethodInfos.Enumerable.Cast.MakeGenericMethod(elementType).Invoke(null, new[] { items });
                    var r2 = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).Invoke(null, new[] { r1 });
                    try
                    {
                        return (IEnumerable<T>)r2;
                    }
                    catch (InvalidCastException)
                    {
                        var enumerable = (System.Collections.IEnumerable)r2;
                        return enumerable.Cast<T>();
                    }
                }
                else
                {
                    // project data record
                    var propertyCount = objects.First().Count;
                    var propertyTypes = new Type[propertyCount];
                    for (int i = 0; i < propertyCount; i++)
                    {
                        var value = objects.Select(record => record.Values.ElementAt(i)).Where(x => !ReferenceEquals(null, x)).FirstOrDefault();
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

                    Func<DynamicObject, object> objectMapper = null;

                    if (constructor != null)
                    {
                        objectMapper = item =>
                        {
                            var values = item.Values.Select((x, i) =>
                            {
                                if (x is DynamicObject)
                                {
                                    // subsequent mapping of nested anonymous type
                                    var targetType = constructor.Parameters[i].ParameterType;
                                    var method = MapDynamicObjectListMethod.MakeGenericMethod(targetType);
                                    var args = new[] { (DynamicObject)x }.AsEnumerable();
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
                            return dataRecord;
                        };
                    }
                    else if (constructors.Any(x => x.Parameters.Length == 0))
                    {
                        constructor = constructors.Single(x => x.Parameters.Length == 0);
                        if (constructor != null)
                        {
                            var properties = elementType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                                .Where(x => x.CanWrite)
                                .ToDictionary(x => x.Name);

                            objectMapper = item =>
                            {
                                if (item.Keys.Any(key => !properties.ContainsKey(key)))
                                {
                                    throw new Exception(string.Format("Failed to set properties of type {0}: ({1})", item.GetType(), string.Join(", ", item.Keys.Where(key => !properties.ContainsKey(key)).ToArray())));
                                }
                                var dataRecord = constructor.Info.Invoke(new object[0]);
                                foreach (var property in properties)
                                {
                                    property.Value.SetValue(dataRecord, item[property.Key]);
                                }
                                return dataRecord;
                            };
                        }
                    }
                    // TODO: suport combinaton of contrcutor an member asignments

                    if (constructor != null && objectMapper != null)
                    {
                        var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                        foreach (var item in objects)
                        {
                            var dataRecord = objectMapper(item);
                            list.Add(dataRecord);
                        }
                        return (IEnumerable<T>)list;
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to pick matching contructor for type {0}", elementType.FullName));
                    }
                }
            }

            throw new Exception(string.Format("Failed to project dynamic objects into type {0}", typeof(T).FullName));
        }

        public static IEnumerable<DynamicObject> Map(object obj)
        {
            IEnumerable<DynamicObject> enumerable;
            if (ReferenceEquals(null, obj))
            {
                //enumerable = new DynamicObject[0];
                enumerable = null;
            }
            else if (obj is IEnumerable<DynamicObject>)
            {
                // cast
                enumerable = (IEnumerable<DynamicObject>)obj;
            }
            else if (IEnumerableExtensions.Any(obj as System.Collections.IEnumerable))
            {
                // put collection into dynamic object collection
                var elementType = TypeHelper.GetElementType(obj.GetType());
                if (elementType.IsEnum() || elementType.IsValueType() || elementType == typeof(string) || elementType.IsArray)
                {
                    enumerable = ((System.Collections.IEnumerable)obj)
                        .OfType<object>()
                        .Select(MapSingle);
                }
                else
                {
                    var properties = elementType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(x => x.CanRead)
                        .ToList();
                    enumerable = ((System.Collections.IEnumerable)obj)
                        .OfType<object>()
                        .Select(x =>
                        {
                            var dynamicObject = new DynamicObject();
                            foreach (var property in properties)
                            {
                                var value = property.GetValue(x);
                                value = MapValueIfRequired(value);
                                dynamicObject[property.Name] = value;
                            }
                            return dynamicObject;
                        });
                }
            }
            else
            {
                // put single object into dynamic object
                var value = MapSingle(obj);
                enumerable = new[] { value };
            }
            var list = ReferenceEquals(null,enumerable) ? null : enumerable.ToList();
            return list;
        }

        public static DynamicObject MapSingle(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
                //return new DynamicObject { { string.Empty, null } };
            }
            if (obj is DynamicObject)
            {
                return (DynamicObject)obj;
            }
            var type = obj.GetType();
            if (type.IsEnum() || type.IsValueType() || type == typeof(string))
            {
                return new DynamicObject { { string.Empty, obj } };
            }
            if (type.IsArray)
            {
                var list = ((System.Collections.IEnumerable)obj)
                    .OfType<object>()
                    .Select(MapValueIfRequired)
                    .ToArray();
                //return !list.Any() ? null : new DynamicObject { { string.Empty, list } };
                return new DynamicObject { { string.Empty, list.Any() ? list : null } };
            }
            var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.CanRead)
                .ToList();
            var dynamicObject = new DynamicObject();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                value = MapValueIfRequired(value);
                dynamicObject[property.Name] = value;
            }
            return dynamicObject;
        }

        private static object MapValueIfRequired(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
            }
            if (obj is DynamicObject || obj is string)
            {
                return obj;
            }
            var type = obj.GetType();
            if (type.IsEnum() || type.IsValueType())
            {
                return obj;
            }
            if (type.IsArray)
            {
                var list = ((System.Collections.IEnumerable)obj)
                    .OfType<object>()
                    .Select(MapValueIfRequired)
                    .ToArray();
                //return new DynamicObject { { string.Empty, list } };
                return list;
            }
            return MapSingle(obj);
        }
    }
}
