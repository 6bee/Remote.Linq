// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using MethodInfo = System.Reflection.MethodInfo;

namespace Remote.Linq.Dynamic
{
    public static partial class DynamicObjectMapper
    {
        internal sealed class ObjectFormatterContext<TFrom, TTo>
        {
            public ObjectFormatterContext(bool suppressTypeInformation = false)
            {
                SuppressTypeInformation = suppressTypeInformation;
                ReferenceMap = new Dictionary<TFrom, TTo>(ObjectReferenceEqualityComparer<TFrom>.Instance);
            }

            internal readonly bool SuppressTypeInformation;
            private readonly IDictionary<TFrom, TTo> ReferenceMap;

            /// <summary>
            /// Returns an existing instance if found in the reference map, creates a new instance otherwise
            /// </summary>
            internal TTo TryGetOrCreateNew(Type objectType, TFrom from, Func<Type, TFrom, ObjectFormatterContext<TFrom, TTo>, TTo> factory, Action<Type, TFrom, TTo, ObjectFormatterContext<TFrom, TTo>> initializer = null)
            {
                TTo to;
                if (!ReferenceMap.TryGetValue(from, out to))
                {
                    to = factory(SuppressTypeInformation ? null : objectType, from, this);

                    try
                    {
                        ReferenceMap.Add(from, to);
                    }
                    catch
                    {
                        // detected cyclic reference
                        // can happen for non-serializable types without parameterless constructor, which have cyclic references 
                        return ReferenceMap[from];
                    }

                    if (!ReferenceEquals(null, initializer))
                    {
                        initializer(objectType, from, to, this);
                    }
                }
                return to;
            }
        }

        const string NumericPattern = @"([0-9]*\.?[0-9]+|[0-9]+\.?[0-9]*)([eE][+-]?[0-9]+)?";

        private static readonly Regex _complexNumberParserRegex = new Regex(string.Format("^(?<Re>[+-]?({0}))(?<Sign>[+-])[iI](?<Im>{0})$", NumericPattern), LocalRegexOptions);

        private static readonly Regex _backingFieldRegex = new Regex(@"^(.+\+)?\<(?<name>.+)\>k__BackingField$", LocalRegexOptions);

        private static readonly Type _genericDictionaryType = typeof(Dictionary<,>);

        private static readonly Type _genericKeyValuePairType = typeof(KeyValuePair<,>);

        private static readonly MethodInfo _mapDynamicObjectListMethod = typeof(DynamicObjectMapper)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "Map")
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Where(x => x.MatchParameters(typeof(IEnumerable<DynamicObject>)))
            .Single();

        private static readonly MethodInfo _mapDynamicObjectListInternalMethod = typeof(DynamicObjectMapper)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "Map")
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Where(x => x.MatchParameters(typeof(IEnumerable<DynamicObject>), typeof(ObjectFormatterContext<DynamicObject, object>)))
            .Single();

        private static readonly MethodInfo _mapDynamicObjectMethod = typeof(DynamicObjectMapper)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(x => x.Name == "Map")
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Where(x => x.MatchParameters(typeof(DynamicObject)))
            .Single();

        private static readonly MethodInfo _mapDynamicObjectInternalMethod = typeof(DynamicObjectMapper)
            .GetMethods(BindingFlags.Static | BindingFlags.NonPublic)
            .Where(x => x.Name == "Map")
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Where(x => x.MatchParameters(typeof(DynamicObject), typeof(ObjectFormatterContext<DynamicObject, object>)))
            .Single();

        private static readonly Type[] _nativeTypes = new[]
            {
                typeof(object),

                typeof(string),

                typeof(int),
                typeof(int?),
                typeof(uint),
                typeof(uint?),

                typeof(byte),
                typeof(byte?),
                typeof(sbyte),
                typeof(sbyte?),

                typeof(short),
                typeof(short?),
                typeof(ushort),
                typeof(ushort?),

                typeof(long),
                typeof(long?),
                typeof(ulong),
                typeof(ulong?),

                typeof(float),
                typeof(float?),

                typeof(double),
                typeof(double?),

                typeof(decimal),
                typeof(decimal?),

                typeof(char),
                typeof(char?),

                typeof(bool),
                typeof(bool?),

                typeof(Guid),
                typeof(Guid?),

                typeof(DateTime),
                typeof(DateTime?),

                typeof(TimeSpan),
                typeof(TimeSpan?),

                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),

#if NET
                typeof(System.Numerics.BigInteger),
                typeof(System.Numerics.BigInteger?),

                typeof(System.Numerics.Complex),
                typeof(System.Numerics.Complex?),
#endif
            };

        private static readonly bool IsFormattingNativeTypesAsStringRequired = false;

        private static bool MatchParameters(this MethodInfo x, params Type[] types)
        {
            var parameters = x.GetParameters();
            if (parameters.Length != types.Length) return false;
            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types[i]) return false;
            }
            return true;
        }

        private static MethodInfo GetMapDynamicObjectListMethod(Type type)
        {
            return _mapDynamicObjectListMethod.MakeGenericMethod(type);
        }

        private static MethodInfo GetMapDynamicObjectListInternalMethod(Type type)
        {
            return _mapDynamicObjectListInternalMethod.MakeGenericMethod(type);
        }

        private static MethodInfo GetMapDynamicObjectMethod(Type type)
        {
            return _mapDynamicObjectMethod.MakeGenericMethod(type);
        }

        private static MethodInfo GetMapDynamicObjectInternalMethod(Type type)
        {
            return _mapDynamicObjectInternalMethod.MakeGenericMethod(type);
        }

        private static object InvokeMethod(this MethodInfo method, params object[] args)
        {
            try
            {
                return method.Invoke(null, args);
            }
            catch (System.Reflection.TargetInvocationException ex)
            {
                Exception innerException;
                if (ReferenceEquals(null, ex.InnerException))
                {
                    innerException = ex;
                }
                else if (ReferenceEquals(null, ex.InnerException.InnerException))
                {
                    innerException = ex.InnerException;
                }
                else
                {
                    innerException = ex.InnerException.InnerException;
                }
                throw new Exception(innerException.Message, innerException);
            }
        }

        public static IEnumerable<object> Map(Type type, IEnumerable<DynamicObject> objects)
        {
            var mapper = GetMapDynamicObjectListMethod(type);
            var result = mapper.InvokeMethod(objects);
            return (IEnumerable<object>)result;
        }

        private static IEnumerable<object> MapDynamicObjectListInternal(Type type, IEnumerable<DynamicObject> objects, ObjectFormatterContext<DynamicObject, object> context)
        {
            var mapper = GetMapDynamicObjectListMethod(type);
            var result = mapper.InvokeMethod(objects, context);
            return (IEnumerable<object>)result;
        }

        public static object Map(Type type, DynamicObject obj)
        {
            var mapper = GetMapDynamicObjectMethod(type);
            var result = mapper.InvokeMethod(obj);
            return result;
        }

        private static object MapDynamicObjectInternal(Type type, DynamicObject obj, ObjectFormatterContext<DynamicObject, object> context)
        {
            var mapper = GetMapDynamicObjectInternalMethod(type);
            var result = mapper.InvokeMethod(obj, context);
            return result;
        }

        public static T Map<T>(DynamicObject obj)
        {
            return Map<T>(obj, new ObjectFormatterContext<DynamicObject, object>());
        }

        private static T Map<T>(DynamicObject obj, ObjectFormatterContext<DynamicObject, object> context)
        {
            return Map<T>(new[] { obj }, context).SingleOrDefault();
        }

        public static object Map(DynamicObject obj)
        {
            if (ReferenceEquals(null, obj)) throw new ArgumentNullException("obj");
            if (ReferenceEquals(null, obj.Type)) throw new InvalidOperationException("Type property must not be null");
            return Map(obj, new ObjectFormatterContext<DynamicObject, object>());
        }

        private static object Map(DynamicObject obj, ObjectFormatterContext<DynamicObject, object> context)
        {
            var result = MapDynamicObjectInternal(obj.Type, obj, context);
            return result;
        }

        public static IEnumerable<T> Map<T>(IEnumerable<DynamicObject> objects)
        {
            return Map<T>(objects, new ObjectFormatterContext<DynamicObject, object>());
        }

        private static IEnumerable<T> Map<T>(IEnumerable<DynamicObject> objects, ObjectFormatterContext<DynamicObject, object> context)
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
            if (objects.All(item => item.MemberCount == 1 && string.IsNullOrEmpty(item.MemberNames.Single())))
            {
                // project single property
                //var items = objects.SelectMany(i => i.Values).Where(x => !ReferenceEquals(null, x)).ToArray();
                var items = objects
                    .SelectMany(i => i.Values)
                    .Select(x => MapDynamicObjectIfRequired(elementType, x, context))
                    .ToArray();
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
                Func<Type, DynamicObject, ObjectFormatterContext<DynamicObject, object>, object> factory;
                Action<Type, DynamicObject, object, ObjectFormatterContext<DynamicObject, object>> initializer = null;
                if (elementType.IsSerializable())
                {
                    factory = (type, item, map) => GetUninitializedObject(type);
                    initializer = PopulateObjectMembers;
                }
                else
                {
                    var propertyCount = objects.First().MemberCount;
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
                            if (ReferenceEquals(null, propertyType) || propertyType == typeof(DynamicObject)) continue;
                            var parameterType = ctor.Parameters[i];
                            if (!parameterType.ParameterType.IsAssignableFrom(propertyType)) return false;
                        }
                        return true;
                    });

                    if (constructor != null)
                    {
                        factory = (type, item, map) =>
                        {
                            var parameterValues = item.Values
                                .Select((x, i) =>
                                {
                                    var parameterType = constructor.Parameters[i].ParameterType;
                                    return MapDynamicObjectIfRequired(parameterType, x, map);
                                })
                                .ToArray();
                            var obj = constructor.Info.Invoke(parameterValues);
                            return obj;
                        };
                    }
                    // TODO: reverse order - try use parameterless constructor first - to support cyclic references where possible
                    else if (constructors.Any(x => x.Parameters.Length == 0))
                    {
                        constructor = constructors.Single(x => x.Parameters.Length == 0);
                        var properties = elementType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(x => x.CanWrite && x.GetIndexParameters().Length == 0)
                            .ToList();
                        factory = (type, item, map) =>
                        {
                            var obj = constructor.Info.Invoke(new object[0]);
                            return obj;
                        };
                        initializer = (type, item, obj, map) =>
                        {
                            // silently skipping values with no matching writable property
                            var memberNames = item.MemberNames;
                            foreach (var property in properties.Where(p => memberNames.Contains(p.Name)))
                            {
                                var value = MapDynamicObjectIfRequired(property.PropertyType, item[property.Name], map);
                                property.SetValue(obj, value);
                            }
                        };
                    }
                    else
                    {
                        throw new Exception(string.Format("Failed to pick matching contructor for type {0}", elementType.FullName));
                    }
                    // TODO: support combination of constructor and member asignments or provide API with registration hook
                }

                var list = (System.Collections.IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                foreach (var item in objects)
                {
                    var obj = context.TryGetOrCreateNew(elementType, item, factory, initializer);
                    list.Add(obj);

                }
                return (IEnumerable<T>)list;
            }

            throw new Exception(string.Format("Failed to project dynamic objects into type {0}", typeof(T).FullName));
        }

        private static object MapDynamicObjectIfRequired(Type targetType, object obj, ObjectFormatterContext<DynamicObject, object> context)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
            }

            var dynamicObj = obj as DynamicObject;
            if (!ReferenceEquals(null, dynamicObj))
            {
                // subsequent mapping of nested dynamic object
                if (!context.SuppressTypeInformation && !ReferenceEquals(null, dynamicObj.Type) && targetType.IsAssignableFrom(dynamicObj.Type.Type))
                {
                    targetType = dynamicObj.Type.Type;
                }

                var mappedValue = MapDynamicObjectInternal(targetType, dynamicObj, context);
                return mappedValue;
            }

            if (obj is System.Collections.IEnumerable && !(obj is string))
            {
                var elementType = TypeHelper.GetElementType(targetType);
                var items = ((System.Collections.IEnumerable)obj).OfType<object>()
                    .Select(x => MapDynamicObjectIfRequired(elementType, x, context))
                    .ToList();
                var r1 = MethodInfos.Enumerable.Cast.MakeGenericMethod(elementType).Invoke(null, new[] { items });

                if (targetType.IsArray)
                {
                    var r2 = MethodInfos.Enumerable.ToArray.MakeGenericMethod(elementType).Invoke(null, new[] { r1 });
                    return r2;
                }

                if (IsMatchingDictionary(targetType, elementType))
                {
                    var targetTypeGenericArguments = targetType.GetGenericArguments();
                    var method = typeof(DynamicObjectMapper)
                        .GetMethod("ToDictoonary", BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(targetTypeGenericArguments.ToArray());
                    var r2 = method.Invoke(null, new object[] { r1 });
                    return r2;
                }

                {
                    var r2 = MethodInfos.Enumerable.ToList.MakeGenericMethod(elementType).Invoke(null, new[] { r1 });
                    return r2;
                }
            }

            if (targetType.IsEnum())
            {
                var objectType = obj.GetType();
                if (objectType.IsEnum())
                {
                    if (objectType.Equals(targetType))
                    {
                        return obj;
                    }
                    // TODO: else we could convert by string or numeric value
                }

                if (obj is string)
                {
                    var enumValue = Enum.Parse(targetType, (string)obj, true);
                    return enumValue;
                }

                {
                    var enumValue = Enum.ToObject(targetType, obj);
                    return enumValue;
                }
            }

            if (obj is string && targetType.IsNativeType())
            {
                return ParseToNativeType(targetType, (string)obj);
            }

            return obj;
        }

        private static object ParseToNativeType(Type targetType, string value)
        {
            if (targetType == typeof(string))
            {
                return value;
            }

            if (targetType == typeof(int) || targetType == typeof(int?))
            {
                return int.Parse(value);
            }

            if (targetType == typeof(uint) || targetType == typeof(uint?))
            {
                return uint.Parse(value);
            }

            if (targetType == typeof(byte) || targetType == typeof(byte?))
            {
                return byte.Parse(value);
            }

            if (targetType == typeof(sbyte) || targetType == typeof(sbyte?))
            {
                return sbyte.Parse(value);
            }

            if (targetType == typeof(short) || targetType == typeof(short))
            {
                return short.Parse(value);
            }

            if (targetType == typeof(ushort) || targetType == typeof(ushort?))
            {
                return ushort.Parse(value);
            }

            if (targetType == typeof(long) || targetType == typeof(long?))
            {
                return long.Parse(value);
            }

            if (targetType == typeof(ulong) || targetType == typeof(ulong?))
            {
                return ulong.Parse(value);
            }

            if (targetType == typeof(float) || targetType == typeof(float?))
            {
                return float.Parse(value);
            }

            if (targetType == typeof(double) || targetType == typeof(double?))
            {
                return double.Parse(value);
            }

            if (targetType == typeof(decimal) || targetType == typeof(decimal?))
            {
                return decimal.Parse(value);
            }

            if (targetType == typeof(char) || targetType == typeof(char?))
            {
#if NETFX_CORE || SILVERLIGHT
                char character;
                if (!char.TryParse(value, out character))
                {
                    throw new FormatException(string.Format("Value '{0}' cannot be parsed into character.", value));
                }
                return character;
#else
                return char.Parse(value);
#endif
            }

            if (targetType == typeof(bool) || targetType == typeof(bool?))
            {
                return bool.Parse(value);
            }

            if (targetType == typeof(Guid) || targetType == typeof(Guid?))
            {
#if NET35
                return new Guid(value);
#else
                return Guid.Parse(value);
#endif
            }

            if (targetType == typeof(DateTime) || targetType == typeof(DateTime?))
            {
                return DateTime.Parse(value);
            }

            if (targetType == typeof(DateTimeOffset) || targetType == typeof(DateTimeOffset?))
            {
                return DateTimeOffset.Parse(value);
            }

            if (targetType == typeof(TimeSpan) || targetType == typeof(TimeSpan?))
            {
                return TimeSpan.Parse(value);
            }
#if NET
            if (targetType == typeof(System.Numerics.BigInteger) || targetType == typeof(System.Numerics.BigInteger?))
            {
                return System.Numerics.BigInteger.Parse(value);
            }

            if (targetType == typeof(System.Numerics.Complex) || targetType == typeof(System.Numerics.Complex?))
            {
                var m = _complexNumberParserRegex.Match(value);
                if (m.Success)
                {
                    var re = double.Parse(m.Groups["Re"].Value);
                    var im = double.Parse(m.Groups["Sign"].Value + m.Groups["Im"].Value);
                    return new System.Numerics.Complex(re, im);
                }
                else
                {
                    throw new FormatException(string.Format("Value '{0}' cannot be parsed into complex number.", value));
                }
            }
#endif
            if (targetType == typeof(object))
            {
                var newObject = new object();
                if (string.Compare(value, string.Empty) == 0 || string.Compare(value, newObject.ToString()) == 0)
                {
                    return newObject;
                }

                throw new FormatException(string.Format(
                    "Value '{0}' cannot be parsed into object. Creating instance of type {1} is only allowed for empty strings and '{2}'.",
                    value, typeof(object), newObject.ToString()));
            }

            throw new NotImplementedException(string.Format("string parser for type {0} is not implemented", targetType));
        }

        private static bool IsMatchingDictionary(Type targetType, Type elementType)
        {
            if (!(targetType.IsGenericType() && elementType.IsGenericType()))
            {
                return false;
            }

            var elementTypeGenericTypeDefinition = elementType.GetGenericTypeDefinition();
            if (!_genericKeyValuePairType.IsAssignableFrom(elementTypeGenericTypeDefinition))
            {
                return false;
            }

            var targetTypeGenericArgumentsCount = targetType.GetGenericArguments().Count();
            if (targetTypeGenericArgumentsCount != 2)
            {
                return false;
            }

            var elementTypeGenericArguments = elementType.GetGenericArguments().ToArray();
            var dictionaryType = _genericDictionaryType.MakeGenericType(elementTypeGenericArguments);
            if (!targetType.IsAssignableFrom(dictionaryType))
            {
                return false;
            }

            return true;
        }

        private static object ToDictoonary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.ToDictionary(x => x.Key, x => x.Value);
        }

        /// <summary>
        /// Maps an instance of <see cref="System.Object"/> or collection of <see cref="System.Object"/> to a collection of <see cref="Remote.Linq.Dynamic.DynamicObject"/>
        /// </summary>
        public static IEnumerable<DynamicObject> Map(object obj, bool suppressTypeInformation = false)
        {
            return Map(obj, new ObjectFormatterContext<object, DynamicObject>(suppressTypeInformation));
        }

        /// <summary>
        /// Maps an instance of collection of objects to a collection of dynamic objects
        /// </summary>
        private static IEnumerable<DynamicObject> Map(object obj, ObjectFormatterContext<object, DynamicObject> context)
        {
            IEnumerable<DynamicObject> enumerable;
            if (ReferenceEquals(null, obj))
            {
                enumerable = null;
            }
            else if (obj is IEnumerable<DynamicObject>)
            {
                // cast
                enumerable = (IEnumerable<DynamicObject>)obj;
            }
            else if (obj is System.Collections.IEnumerable && !(obj is string))
            {
                enumerable = ((System.Collections.IEnumerable)obj)
                    .Cast<object>()
                    .Select(x => MapSingle(x, context));
            }
            else
            {
                // put single object into dynamic object
                var value = MapSingle(obj, context);
                enumerable = new[] { value };
            }
            var list = ReferenceEquals(null, enumerable) ? null : enumerable.ToList();
            return list;
        }

        /// <summary>
        /// Maps an <see cref="Object"/> to a <see cref="DynamicObject"/>
        /// </summary>
        /// <remarks>Null references and dynamic objects are not mapped.</remarks>
        public static DynamicObject MapSingle(object obj, bool suppressTypeInformation = false)
        {
            return MapSingle(obj, new ObjectFormatterContext<object, DynamicObject>(suppressTypeInformation));
        }

        /// <summary>
        /// Maps an object to a dynamic object
        /// </summary>
        /// <remarks>Null references and dynamic objects are not mapped.</remarks>
        private static DynamicObject MapSingle(object obj, ObjectFormatterContext<object, DynamicObject> context)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
            }

            if (obj is DynamicObject)
            {
                return (DynamicObject)obj;
            }

            Func<Type, object, ObjectFormatterContext<object, DynamicObject>, DynamicObject> facotry;
            Action<Type, object, DynamicObject, ObjectFormatterContext<object, DynamicObject>> initializer = null;

            var type = obj.GetType();
            if (type.IsNativeType())
            {
                facotry = (t, o, m) =>
                {
                    var value = MapValueIfRequired(o, m);
                    return new DynamicObject(t) { { string.Empty, value } };
                };
            }
            else if (type.IsArray)
            {
                facotry = (t, o, m) =>
                {
                    var list = ((System.Collections.IEnumerable)o)
                        .OfType<object>()
                        .Select(x => MapValueIfRequired(x, m))
                        .ToArray();
                    return new DynamicObject(t) { { string.Empty, list.Any() ? list : null } };
                };
            }
            else
            {
                facotry = (t, o, m) => new DynamicObject(t);
                initializer = PopulateObjectMembers;
            }

            return context.TryGetOrCreateNew(type, obj, facotry, initializer);
        }

        private static bool IsNativeType(this Type type)
        {
            return _nativeTypes.Contains(type);
        }

        /// <summary>
        /// Maps from object to dynamic object if required.
        /// </summary>
        /// <remarks>Null references, strings, value types, and dynamic objects are no mapped.</remarks>
        private static object MapValueIfRequired(object obj, ObjectFormatterContext<object, DynamicObject> context)
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

            if (type.IsNativeType())
            {
                return IsFormattingNativeTypesAsStringRequired ? FormatNativeTypeAsString(obj, type) : obj;
            }

            if (type.IsEnum())
            {
                return obj.ToString();
            }

            if (obj is System.Collections.IEnumerable)
            {
                var list = ((System.Collections.IEnumerable)obj)
                    .OfType<object>()
                    .Select(x => MapValueIfRequired(x, context))
                    .ToArray();
                return list;
            }

            return MapSingle(obj, context);
        }

        private static object FormatNativeTypeAsString(object obj, Type type)
        {
            if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return ((DateTime)obj).ToString("o");
            }

            if (type == typeof(float) || type == typeof(float?))
            {
                return ((float)obj).ToString("R");
            }

            if (type == typeof(double) || type == typeof(double?))
            {
                return ((double)obj).ToString("R");
            }
#if NET
            if (type == typeof(System.Numerics.BigInteger) || type == typeof(System.Numerics.BigInteger?))
            {
                return ((DateTime)obj).ToString("R");
            }

            if (type == typeof(System.Numerics.Complex) || type == typeof(System.Numerics.Complex?))
            {
                var c = (System.Numerics.Complex)obj;
                return string.Format("{0:R}{1:+;-}i{2:R}", c.Real, Math.Sign(c.Imaginary), Math.Abs(c.Imaginary));
            }
#endif
            return obj.ToString();
        }

        /// <summary>
        /// Extrancts member values from source object and populates to dynamic object 
        /// </summary>
        private static void PopulateObjectMembers(Type type, object from, DynamicObject to, ObjectFormatterContext<object, DynamicObject> context)
        {
            //var type = to.Type.Type;

            // TODO: add support for ISerializable
            // TODO: add support for OnSerializingAttribute, OnSerializedAttribute, OnDeserializingAttribute, OnDeserializedAttribute
            if (type.IsSerializable())
            {
                MapObjectMembers(from, to, context);
            }
            else
            {
                // TODO: should fields be supported too?
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.CanRead && x.GetIndexParameters().Length == 0);
                foreach (var property in properties)
                {
                    var value = property.GetValue(from);
                    value = MapValueIfRequired(value, context);
                    to[property.Name] = value;
                }
            }
        }
    }
}
