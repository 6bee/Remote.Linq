// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TypeSystem.Emit
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    partial class TypeEmitter
    {
        private sealed class PropertyList
        {
            private readonly ReadOnlyCollection<string> _properties;

            private readonly Lazy<int> _hash;

            internal PropertyList(IEnumerable<string> properties)
            {
                _properties = properties.ToList().AsReadOnly();
                _hash = new Lazy<int>(() =>
                {
                    unchecked
                    {
                        var hash = 27;
                        foreach (var property in _properties)
                        {
                            hash = (hash * 13) + property.GetHashCode();
                        }

                        return hash;
                    }
                });
            }

            public IEnumerable<string> Properties
            {
                get { return _properties; }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() == typeof(PropertyList))
                {
                    return Equals((PropertyList)obj);
                }

                return false;
            }

            private bool Equals(PropertyList other)
            {
                if (_properties.Count != other._properties.Count)
                {
                    return false;
                }

                for (int i = 0; i < _properties.Count; i++)
                {
                    if (!string.Equals(_properties[i], other._properties[i], StringComparison.Ordinal))
                    {
                        return false;
                    }
                }

                return true;
            }

            public override int GetHashCode()
            {
                return _hash.Value;
            }
        }

        private sealed class TypeWithPropertyList
        {
            private readonly string _typeFullName;

            private readonly ReadOnlyCollection<Tuple<string, Type>> _properties;

            private readonly Lazy<int> _hash;

            public TypeWithPropertyList(TypeInfo typeInfo)
            {
                _typeFullName = typeInfo.FullName;

                var properties = typeInfo.Properties;
                _properties = ReferenceEquals(null, properties)
                    ? new List<Tuple<string, Type>>().AsReadOnly()
                    : properties.Select(CreatePropertyInfo).ToList().AsReadOnly();

                _hash = new Lazy<int>(() =>
                {
                    unchecked
                    {
                        var hash = 27;
                        foreach (var property in _properties)
                        {
                            hash = (hash * 13) + property.GetHashCode();
                        }

                        return hash;
                    }
                });
            }

            public string TypeFullName
            {
                get { return _typeFullName; }
            }

            public IEnumerable<Tuple<string, Type>> Properties
            {
                get { return _properties; }
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }

                if (ReferenceEquals(this, obj))
                {
                    return true;
                }

                if (obj.GetType() == typeof(TypeWithPropertyList))
                {
                    return Equals((TypeWithPropertyList)obj);
                }

                return false;
            }

            private bool Equals(TypeWithPropertyList other)
            {
                if (!string.Equals(_typeFullName, other._typeFullName))
                {
                    return false;
                }

                if (_properties.Count != other._properties.Count)
                {
                    return false;
                }

                var match = other._properties.All(x => _properties.Contains(x));
                if (!match)
                {
                    return false;
                }

                return true;
            }

            public override int GetHashCode()
            {
                return _hash.Value;
            }

            private static Tuple<string, Type> CreatePropertyInfo(PropertyInfo propertyInfo)
            {
                if (ReferenceEquals(null, propertyInfo))
                {
                    return null;
                }

                var propertyName = propertyInfo.Name;
                if (string.IsNullOrEmpty(propertyName))
                {
                    throw new Exception("Property name missing");
                }

                var propertyTypeInfo = propertyInfo.PropertyType;
                if (ReferenceEquals(null, propertyInfo))
                {
                    throw new Exception(string.Format("Property type missing for property '{0}'", propertyInfo.Name));
                }

                var propertyType = propertyTypeInfo.Type;

                return new Tuple<string, Type>(propertyName, propertyType);
            }
        }

        private sealed class TypeCache : TransparentCache<object, Type>
        {
            internal Type GetOrCreate(IEnumerable<string> properties, Func<IEnumerable<string>, Type> factory)
            {
                var key = new PropertyList(properties);
                return base.GetOrCreate(key, x => factory(properties));
            }

            internal Type GetOrCreate(TypeInfo typeInfo, Func<TypeInfo, Type> factory)
            {
                var key = new TypeWithPropertyList(typeInfo);
                return base.GetOrCreate(key, x => factory(typeInfo));
            }
        }
    }
}
