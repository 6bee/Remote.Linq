// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Remote.Linq.TypeSystem.Emit
{
    partial class TypeEmitter
    {
        private sealed class PropertyList
        {
            internal readonly ReadOnlyCollection<string> Properties;

            private readonly Lazy<int> _hash;

            internal PropertyList(IEnumerable<string> properties)
            {
                Properties = properties.ToList().AsReadOnly();
                _hash = new Lazy<int>(() =>
                {
                    var hash = 27;
                    foreach (var property in Properties)
                    {
                        hash = unchecked((hash * 13) + property.GetHashCode());
                    }
                    return hash;
                });
            }

            public override bool Equals(object obj)
            {
                var other = (PropertyList)obj;
                if (Properties.Count != other.Properties.Count)
                {
                    return false;
                }
                for (int i = 0; i < Properties.Count; i++)
                {
                    if (string.Compare(Properties[i], other.Properties[i]) != 0)
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

        private sealed class TypeCache : TransparentCache<PropertyList, Type>
        {
            internal Type GetOrCreate(IEnumerable<string> properties, Func<IEnumerable<string>, Type> factory)
            {
                var key = new PropertyList(properties);
                return base.GetOrCreate(key, x => factory(x.Properties));
            }
        }
    }
}
