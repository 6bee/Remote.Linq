// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [Serializable]
    [KnownType(typeof(object))]
    [KnownType(typeof(object[]))]
    public partial class DynamicObject : Dictionary<string, object>
    {
        public DynamicObject()
        {
        }

        public DynamicObject(Type type)
            : this(new TypeInfo(type))
        {
        }

        public DynamicObject(TypeInfo type)
        {
            Type = type;
        }

        public DynamicObject(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var item in data)
            {
                Add(item.Key, item.Value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }

        public object Set(string name, object value)
        {
            this[name] = value;
            return value;
        }

        public object Get(string name)
        {
            object value;
            if (!this.TryGetValue(name, out value))
            {
                value = null;
            }
            return value;
        }
    }
}
