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
    public partial class DynamicObjectMapper : IDynamicObjectMapper
    {
        private sealed class ObjectFormatterContext<TFrom, TTo>
        {
            private readonly Dictionary<TFrom, TTo> ReferenceMap = new Dictionary<TFrom, TTo>(ObjectReferenceEqualityComparer<TFrom>.Instance);

            /// <summary>
            /// Returns an existing instance if found in the reference map, creates a new instance otherwise
            /// </summary>
            internal TTo TryGetOrCreateNew(Type objectType, TFrom from, Func<Type, TFrom, TTo> factory, Action<Type, TFrom, TTo> initializer)
            {
                TTo to;
                if (!ReferenceMap.TryGetValue(from, out to))
                {
                    to = factory(objectType, from);

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
                        initializer(objectType, from, to);
                    }
                }
                return to;
            }

            /// <summary>
            /// Returns an existing instance if found in the reference map, creates a new instance otherwise
            /// </summary>
            internal TTo TryGetOrCreateNew(Type objectType, TFrom from, Func<Type, TFrom, bool, TTo> factory, Action<Type, TFrom, TTo, bool> initializer, bool setTypeInformation)
            {
                TTo to;
                if (!ReferenceMap.TryGetValue(from, out to))
                {
                    to = factory(setTypeInformation ? objectType : null, from, setTypeInformation);

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
                        initializer(objectType, from, to, setTypeInformation);
                    }
                }
                return to;
            }
        }

        private const string NumericPattern = @"([0-9]*\.?[0-9]+|[0-9]+\.?[0-9]*)([eE][+-]?[0-9]+)?";

        private static readonly Regex _complexNumberParserRegex = new Regex(string.Format("^(?<Re>[+-]?({0}))(?<Sign>[+-])[iI](?<Im>{0})$", NumericPattern), LocalRegexOptions);

        private static readonly Regex _backingFieldRegex = new Regex(@"^(.+\+)?\<(?<name>.+)\>k__BackingField$", LocalRegexOptions);

        private static readonly Type _genericDictionaryType = typeof(Dictionary<,>);

        private static readonly Type _genericKeyValuePairType = typeof(KeyValuePair<,>);

        private static readonly Dictionary<Type, object> _nativeTypes = new[]
            {
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
            }.ToDictionary(x => x, x => (object)null);

        private static readonly MethodInfo _mapDynamicObjectInternalMethod = typeof(DynamicObjectMapper)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
            .Where(x => x.Name == "MapInternal")
            .Where(x => x.IsGenericMethod && x.GetGenericArguments().Length == 1)
            .Where(x => MatchParameters(x, typeof(DynamicObject)))
            .Single();

        private readonly ObjectFormatterContext<DynamicObject, object> _fromContext;
        private readonly ObjectFormatterContext<object, DynamicObject> _toContext;
        private readonly ITypeResolver _typeResolver;
        private readonly Dictionary<Type, object> _knownTypes;

        /// <summary>
        /// Creates a new instance of <see cref="DynamicObjectMapper"/>
        /// </summary>
        /// <param name="typeResolver">Instance of <see cref="ITypeResolver"/> to be used to resolve types</param>
        /// <param name="knownTypes">Types not required to be mapped into <see cref="DynamicObject"/></param>
        public DynamicObjectMapper(ITypeResolver typeResolver = null, IEnumerable<Type> knownTypes = null)
        {
            _fromContext = new ObjectFormatterContext<DynamicObject, object>();
            _toContext = new ObjectFormatterContext<object, DynamicObject>();
            _typeResolver = typeResolver ?? TypeResolver.Instance;
            _knownTypes = ReferenceEquals(null, knownTypes) ? new Dictionary<Type,object>(0) : knownTypes.ToDictionary(x => x, x => (object)null);
        }

        public IEnumerable<object> Map(IEnumerable<DynamicObject> objects, Type type)
        {
            if (ReferenceEquals(null, objects))
            {
                throw new ArgumentNullException("objects");
            }

            var items = objects.Select(x => Map(x, type));

            if (ReferenceEquals(null, type))
            {
                return items.ToArray();
            }
            else
            {
                var r1 = MethodInfos.Enumerable.Cast.MakeGenericMethod(type).Invoke(null, new[] { items });
                var r2 = MethodInfos.Enumerable.ToArray.MakeGenericMethod(type).Invoke(null, new[] { r1 });
                return (IEnumerable<object>)r2;
            }
        }

        /// <summary>
        /// Maps a <see cref="DynamicObject"/> into a collection of objects
        /// </summary>
        /// <param name="obj"><see cref="DynamicObject"/> to be mapped</param>
        /// <param name="targetType">Target type for mapping, set this parameter to null if type information included within <see cref="DynamicObject"/> should be used.</param>
        /// <returns>The object created based on the <see cref="DynamicObject"/> specified</returns>
        public object Map(DynamicObject obj, Type type)
        {
            return MapFromDynamicObjectGraph(obj, type);
        }

        public object Map(DynamicObject obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
            }

            if (ReferenceEquals(null, obj.Type))
            {
                throw new InvalidOperationException("Type property must not be null");
            }

            var type = _typeResolver.ResolveType(obj.Type);
            return Map(obj, type);
        }

        /// <summary>
        /// Maps a <see cref="DynamicObject"/> into an instance of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The target type in which the <see cref="DynamicObject"/> have to be mapped to</typeparam>
        /// <param name="obj"><see cref="DynamicObject"/> to be mapped</param>
        /// <returns>The object created based on the <see cref="DynamicObject"/> specified</returns>
        public T Map<T>(DynamicObject obj)
        {
            return (T)MapFromDynamicObjectGraph(obj, typeof(T));
        }

        /// <summary>
        /// Maps a collection of <see cref="DynamicObject"/>s into a collection of <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">The target type in which the <see cref="DynamicObject"/> have to be mapped to</typeparam>
        /// <param name="objects">Collection of <see cref="DynamicObject"/>s to be mapped</param>
        /// <returns>Collection of <typeparamref name="T"/> created based on the <see cref="DynamicObject"/>s specified</returns>
        public IEnumerable<T> Map<T>(IEnumerable<DynamicObject> objects)
        {
            return objects.Select(x => Map<T>(x)).ToList();
        }

        /// <summary>
        /// Maps a collection of objects into a collection of <see cref="DynamicObject"/>
        /// </summary>
        /// <param name="objects">The objects to be mapped</param>
        /// <param name="setTypeInformation">Set this parameter to true if type information should be included within the <see cref="DynamicObject"/>s, set it to false otherwise.</param>
        /// <returns>A collection of <see cref="DynamicObject"/> representing the objects specified</returns>
        public IEnumerable<DynamicObject> MapCollection(object obj, bool setTypeInformation = true)
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
                    .Select(x => MapObject(x, setTypeInformation));
            }
            else
            {
                // put single object into dynamic object
                var value = MapObject(obj, setTypeInformation);
                enumerable = new[] { value };
            }
            var list = ReferenceEquals(null, enumerable) ? null : enumerable.ToList();
            return list;
        }

        /// <summary>
        /// Mapps the specified instance into a <see cref="DynamicObject"/>
        /// </summary>
        /// <remarks>Null references and <see cref="DynamicObject"/> are not mapped.</remarks>
        /// <param name="obj">The instance to be mapped</param>
        /// <param name="setTypeInformation">Set this parameter to true if type information should be included within the <see cref="DynamicObject"/>, set it to false otherwise.</param>
        /// <returns>An instance of <see cref="DynamicObject"/> representing the mapped instance</returns>
        public DynamicObject MapObject(object obj, bool setTypeInformation = true)
        {
            return MapToDynamicObjectGraph(obj, setTypeInformation);
        }

        protected virtual bool FormatNativeTypesAsString
        {
            get { return false; }
        }

        protected virtual object MapFromDynamicObjectGraph(object obj, Type targetType)
        {
            return MapFromDynamicObjectIfRequired(obj, targetType);
        }

        protected virtual DynamicObject MapToDynamicObjectGraph(object obj, bool setTypeInformation)
        {
            return MapInternal(obj, setTypeInformation);
        }

        private object MapFromDynamicObjectIfRequired(object obj, Type targetType)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
            }

            var dynamicObj = obj as DynamicObject;
            if (!ReferenceEquals(null, dynamicObj))
            {
                // subsequent mapping of nested dynamic object
                if (!ReferenceEquals(null, dynamicObj.Type))
                {
                    var type = _typeResolver.ResolveType(dynamicObj.Type);
                    if (ReferenceEquals(null, targetType) || targetType.IsAssignableFrom(type))
                    {
                        targetType = type;
                    }
                }

                if (IsSingleValueWrapper(dynamicObj))
                {
                    return MapFromDynamicObjectIfRequired(dynamicObj.Values.Single(), targetType);
                }
                
                var mappedValue = MapInternal(dynamicObj, targetType);
                return mappedValue;
            }

            var objectType = obj.GetType();

            if (objectType == targetType)
            {
                return obj;
            }

            if (IsKnownType(objectType))
            {
                return obj;
            }

            if (IsNativeType(targetType))
            {
                return obj is string ? ParseToNativeType(targetType, (string)obj) : obj;
            }

            if (obj is System.Collections.IEnumerable && !(obj is string))
            {
                var elementType = TypeHelper.GetElementType(targetType);
                var items = ((System.Collections.IEnumerable)obj).OfType<object>()
                    .Select(x => MapFromDynamicObjectGraph(x, elementType))
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
                        .GetMethod("ToDictionary", BindingFlags.Static | BindingFlags.NonPublic)
                        .MakeGenericMethod(targetTypeGenericArguments.ToArray());
                    var r2 = method.Invoke(null, new object[] { r1 });
                    return r2;
                }

                if (targetType.IsAssignableFrom(typeof(List<>).MakeGenericType(elementType)))
                {
                    var r2 = MethodInfos.Enumerable.ToList.MakeGenericMethod(elementType).Invoke(null, new[] { r1 });
                    return r2;
                }

                throw new Exception(string.Format("Failed to project collection of {0} into type {1}", elementType, targetType));
            }

            if (targetType.IsEnum())
            {
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

            return obj;
        }

        /// <summary>
        /// Maps an object to a dynamic object
        /// </summary>
        /// <remarks>Null references and dynamic objects are not mapped.</remarks>
        private DynamicObject MapInternal(object obj, bool setTypeInformation)
        {
            if (ReferenceEquals(null, obj))
            {
                return null;
            }

            if (obj is DynamicObject)
            {
                return (DynamicObject)obj;
            }

            Func<Type, object, bool, DynamicObject> facotry;
            Action<Type, object, DynamicObject, bool> initializer = null;

            var type = obj.GetType();
            if (IsNativeType(type) || IsKnownType(type))
            {
                facotry = (t, o, f) =>
                {
                    var value = MapToDynamicObjectIfRequired(o, f);
                    return new DynamicObject(t) { { string.Empty, value } };
                };
            }
            else if (type.IsArray)
            {
                facotry = (t, o, f) =>
                {
                    var list = ((System.Collections.IEnumerable)o)
                        .OfType<object>()
                        .Select(x => MapToDynamicObjectIfRequired(x, f))
                        .ToArray();
                    return new DynamicObject(t) { { string.Empty, list.Any() ? list : null } };
                };
            }
            else
            {
                facotry = (t, o, f) => new DynamicObject(t);
                initializer = PopulateObjectMembers;
            }

            return _toContext.TryGetOrCreateNew(type, obj, facotry, initializer, setTypeInformation);
        }

        /// <summary>
        /// Maps from object to dynamic object if required.
        /// </summary>
        /// <remarks>Null references, strings, value types, and dynamic objects are no mapped.</remarks>
        private object MapToDynamicObjectIfRequired(object obj, bool setTypeInformation)
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

            if (IsKnownType(type))
            {
                return obj;
            }

            if (IsNativeType(type))
            {
                return FormatNativeTypesAsString ? FormatNativeTypeAsString(obj, type) : obj;
            }

            if (type.IsEnum())
            {
                return obj.ToString();
            }

            if (obj is System.Collections.IEnumerable)
            {
                var list = ((System.Collections.IEnumerable)obj)
                    .OfType<object>()
                    .Select(x => MapToDynamicObjectIfRequired(x, setTypeInformation))
                    .ToArray();
                return list;
            }

            return MapToDynamicObjectGraph(obj, setTypeInformation);
        }

        /// <summary>
        /// Extrancts member values from source object and populates to dynamic object 
        /// </summary>
        private void PopulateObjectMembers(Type type, object from, DynamicObject to, bool setTypeInformation)
        {
            // TODO: add support for ISerializable
            // TODO: add support for OnSerializingAttribute, OnSerializedAttribute, OnDeserializingAttribute, OnDeserializedAttribute
            if (type.IsSerializable())
            {
                MapObjectMembers(from, to, setTypeInformation);
            }
            else
            {
                var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Where(x => x.CanRead && x.GetIndexParameters().Length == 0);
                foreach (var property in properties)
                {
                    var value = property.GetValue(from);
                    value = MapToDynamicObjectIfRequired(value, setTypeInformation);
                    to[property.Name] = value;
                }
            }
        }

        private T MapInternal<T>(DynamicObject obj)
        {
            return MapInternal<T>(new[] { obj }).Single();
        }

        private IEnumerable<T> MapInternal<T>(IEnumerable<DynamicObject> objects)
        {
            if (ReferenceEquals(null, objects))
            {
                //return null;
                throw new ArgumentNullException("objects");
            }

            if (!objects.Any())
            {
                return new T[0];
            }

            if (typeof(T).IsAssignableFrom(typeof(DynamicObject)) && typeof(T) != typeof(object))
            {
                return objects.Cast<T>();
            }

            var elementType = TypeHelper.GetElementType(typeof(T));
            if (objects.All(item => ReferenceEquals(null, item) || IsSingleValueWrapper(item)))
            {
                // project single property
                var items = objects
                    .SelectMany(i => ReferenceEquals(null, i) ? new object[] { null } : i.Values)
                    .Select(x => MapFromDynamicObjectGraph(x, elementType))
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
                Func<Type, DynamicObject, object> factory;
                Action<Type, DynamicObject, object> initializer = null;
                if (elementType.IsSerializable())
                {
                    factory = (type, item) => GetUninitializedObject(type);
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
                        factory = (type, item) =>
                        {
                            var parameterValues = item.Values
                                .Select((x, i) =>
                                {
                                    var parameterType = constructor.Parameters[i].ParameterType;
                                    return MapFromDynamicObjectGraph(x, parameterType);
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
                        factory = (type, item) =>
                        {
                            var obj = constructor.Info.Invoke(new object[0]);
                            return obj;
                        };
                        initializer = (type, item, obj) =>
                        {
                            // silently skipping values with no matching writable property
                            var memberNames = item.MemberNames;
                            foreach (var property in properties.Where(p => memberNames.Contains(p.Name)))
                            {
                                var value = MapFromDynamicObjectGraph(item[property.Name], property.PropertyType);
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
                    var obj = ReferenceEquals(null, item) ? null : _fromContext.TryGetOrCreateNew(elementType, item, factory, initializer);
                    list.Add(obj);

                }
                return (IEnumerable<T>)list;
            }

            throw new Exception(string.Format("Failed to project dynamic objects into type {0}", typeof(T).FullName));
        }

        private object MapInternal(DynamicObject obj, Type type)
        {
            var mapper = GetMapInternalMethod(type);
            var result = InvokeMethod(this, mapper, obj);
            return result;
        }

        private bool IsKnownType(Type type)
        {
            return _knownTypes.ContainsKey(type);
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
            throw new NotImplementedException(string.Format("string parser for type {0} is not implemented", targetType));
        }

        private static bool IsSingleValueWrapper(DynamicObject item)
        {
            return (item.MemberCount == 1 && string.IsNullOrEmpty(item.MemberNames.Single()));
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

        private static object ToDictionary<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> items)
        {
            return items.ToDictionary(x => x.Key, x => x.Value);
        }

        private static bool IsNativeType(Type type)
        {
            return _nativeTypes.ContainsKey(type);
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
                return ((System.Numerics.BigInteger)obj).ToString("R");
            }

            if (type == typeof(System.Numerics.Complex) || type == typeof(System.Numerics.Complex?))
            {
                var c = (System.Numerics.Complex)obj;
                return string.Format("{0:R}{1:+;-}i{2:R}", c.Real, Math.Sign(c.Imaginary), Math.Abs(c.Imaginary));
            }
#endif
            return obj.ToString();
        }

        private static bool MatchParameters(MethodInfo x, params Type[] types)
        {
            var parameters = x.GetParameters();
            if (parameters.Length != types.Length)
            {
                return false;
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType != types[i]) return false;
            }

            return true;
        }

        private static MethodInfo GetMapInternalMethod(Type type)
        {
            return _mapDynamicObjectInternalMethod.MakeGenericMethod(type);
        }

        private static object InvokeMethod(DynamicObjectMapper instance, MethodInfo method, params object[] args)
        {
            try
            {
                return method.Invoke(instance, args);
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
    }
}