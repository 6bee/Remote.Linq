// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [DebuggerDisplay("Count = {GetCount()}")]
    [KnownType(typeof(object))]
    [KnownType(typeof(object[]))]
    public partial class DynamicObject : IEnumerable<KeyValuePair<string, object>>
    {
        public DynamicObject()
        {
            Members = new Dictionary<string, object>();
        }

        public DynamicObject(Type type)
            : this(ReferenceEquals(null, type) ? null : new TypeInfo(type))
        {
        }

        public DynamicObject(TypeInfo type)
            : this()
        {
            Type = type;
        }

        public DynamicObject(IEnumerable<KeyValuePair<string, object>> data)
            : this()
        {
            foreach (var item in data)
            {
                Add(item.Key, item.Value);
            }
        }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Dictionary<string, object> Members { get; private set; }

        public object this[string name]
        {
            get { return Members[name]; }
            set { Members[name] = value; }
        }

        public int GetCount()
        {
            return Members.Count;
        }

        public IEnumerable<string> GetMemberNames()
        {
            return Members.Keys.ToList();
        }

        public IEnumerable<object> GetValues()
        {
            return Members.Values.ToList();
        }

        public object Set(string name, object value)
        {
            this[name] = value;
            return value;
        }

        public object Get(string name)
        {
            object value;
            if (!TryGet(name, out value))
            {
                value = null;
            }
            return value;
        }

        public void Add(string name, object value)
        {
            Members.Add(name, value);
        }

        public bool Remove(string name)
        {
            return Members.Remove(name);
        }

        public bool TryGet(string name, out object value)
        {
            return Members.TryGetValue(name, out value);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return Members.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
