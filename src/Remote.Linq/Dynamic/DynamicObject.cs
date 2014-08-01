// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [Serializable]
    //[DataContract(IsReference = true)]
    [KnownType(typeof(object))]
    [KnownType(typeof(object[]))]
    public sealed partial class DynamicObject : Dictionary<string, object>
    {
        public DynamicObject()
        {
        }

        public DynamicObject(IEnumerable<KeyValuePair<string, object>> data)
        {
            foreach (var item in data)
            {
                Add(item.Key, item.Value);
            }
        }

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
