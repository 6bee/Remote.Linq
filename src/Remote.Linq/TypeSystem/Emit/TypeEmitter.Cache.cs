// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Remote.Linq.TypeSystem.Emit
{
    partial class TypeEmitter
    {
        private sealed class TypeCache
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
                        var hash = 27;
                        foreach (var property in _properties)
                        {
                            hash = unchecked((hash * 13) + property.GetHashCode());
                        }
                        return hash;
                    });
                }

                public override bool Equals(object obj)
                {
                    var other = (PropertyList)obj;
                    if (_properties.Count != other._properties.Count)
                    {
                        return false;
                    }
                    for (int i = 0; i < _properties.Count; i++)
                    {
                        if (string.Compare(_properties[i], other._properties[i]) != 0)
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

            private readonly Dictionary<PropertyList, WeakReference> _cache = new Dictionary<PropertyList, WeakReference>();

            internal Type GetOrCreate(IEnumerable<string> properties, Func<IEnumerable<string>, Type> factory)
            {
                var key = new PropertyList(properties);
                Type type = null;
                lock (_cache)
                {
                    // probe cache
                    WeakReference typeref;
                    if (_cache.TryGetValue(key, out typeref))
                    {
                        type = (Type)typeref.Target;
                    }
                    // emit type if not found in cache 
                    if (ReferenceEquals(null, type))
                    {
                        type = factory(properties);
                        _cache[key] = new WeakReference(type);
                    }
                }
                // clean-up stale references from cache
                Task.Run(delegate
                {
                    lock (_cache)
                    {
                        foreach (var iten in _cache.Where(x => !x.Value.IsAlive).ToArray())
                        {
                            _cache.Remove(iten.Key);
                        }
                    }
                });
                return type;
            }
        }
    }
}
