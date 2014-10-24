// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Dynamic
{
    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("Count = {MemberCount}")]
    public partial class DynamicObject : IEnumerable<DynamicObject.Property>, IDictionary<string, object>
    {
        [Serializable]
        [DataContract(IsReference = true)]
        [KnownType(typeof(object))]
        [KnownType(typeof(object[]))]
        [DebuggerDisplay("{Name} = {Value}")]
        public class Property
        {
            public Property()
            {
            }

            public Property(string name, object value)
            {
                Name = name;
                Value = value;
            }

            internal Property(KeyValuePair<string, object> item)
                : this(item.Key, item.Value)
            {
            }

            [DataMember(Order = 1)]
            public string Name { get; set; }

            [DataMember(Order = 2)]
            public object Value { get; set; }

            public static implicit operator KeyValuePair<string, object>(Property p)
            {
                return new KeyValuePair<string, object>(p.Name, p.Value);
            }
        }

        private readonly static object _lock = new object();
        private Dictionary<string, object> _data;

        /// <summary>
        /// Creates a new instance of a dynamic object
        /// </summary>
        public DynamicObject()
        {
        }

        /// <summary>
        /// Creates a new instance of a dynamic object, setting the specified type
        /// </summary>
        /// <param name="type">The type to be set</param>
        public DynamicObject(Type type)
            : this(ReferenceEquals(null, type) ? null : new TypeInfo(type))
        {
        }

        /// <summary>
        /// Creates a new instance of a dynamic object, setting the specified type
        /// </summary>
        /// <param name="type">The type to be set</param>
        public DynamicObject(TypeInfo type)
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of a dynamic object, setting the specified members
        /// </summary>
        /// <param name="members">Initial collection of properties and values</param>
        /// <exception cref="ArgumentNullException">The specified members collection is null</exception>
        public DynamicObject(IEnumerable<KeyValuePair<string, object>> members)
        {
            if (ReferenceEquals(null, members))
            {
                throw new ArgumentNullException("data");
            }

            foreach (var item in members)
            {
                Add(item.Key, item.Value);
            }
        }

        /// <summary>
        /// Creates a new instance of a dynamic object, setting the specified members
        /// </summary>
        /// <param name="members">Initial collection of properties and values</param>
        /// <exception cref="ArgumentNullException">The specified members collection is null</exception>
        public DynamicObject(IEnumerable<Property> members)
        {
            if (ReferenceEquals(null, members))
            {
                throw new ArgumentNullException("data");
            }

            foreach (var item in members)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Creates a new instance of a dynamic object, representing the object structure defined by the specified object
        /// </summary>
        /// <param name="obj">The object to be represented by the new dynamic object</param>
        /// <param name="mapper">Optional instance of dynamic object mapper</param>
        /// <exception cref="ArgumentNullException">The specified object is null</exception>
        public DynamicObject(object obj, IDynamicObjectMapper mapper = null)
        {
            if (ReferenceEquals(null, obj))
            {
                throw new ArgumentNullException("obj");
            }

            var dynamicObject = (mapper ?? new DynamicObjectMapper()).MapObject(obj);
            Type = dynamicObject.Type;
            _data = dynamicObject.Data;
        }

        private Dictionary<string, object> Data
        {
            get
            {
                if (ReferenceEquals(null, _data))
                {
                    lock (_lock)
                    {
                        if (ReferenceEquals(null, _data))
                        {
                            _data = new Dictionary<string, object>();
                        }
                    }
                }
                return _data;
            }
        }

        /// <summary>
        /// Gets or sets the type of object represented by this dynamic object instance
        /// </summary>
        [DataMember(Order = 1)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 2)]
        public IEnumerable<Property> Members
        {
            get
            {
                return Data.Select(x => new Property(x)).ToList();
            }
            set
            {
                _data = value.ToDictionary(x => x.Name, x => x.Value);
            }
        }

        /// <summary>
        /// Gets the count of members (dynamically added properties) hold by this dynamic object
        /// </summary>
        public int MemberCount
        {
            get { return Data.Count; }
        }

        /// <summary>
        /// Gets a collection of member names hold by this dynamic object
        /// </summary>
        public IEnumerable<string> MemberNames
        {
            get { return Data.Keys; }
        }

        /// <summary>
        /// Gets a collection of member values hold by this dynamic object
        /// </summary>
        public IEnumerable<object> Values
        {
            get { return Data.Values; }
        }

        /// <summary>
        /// Gets or sets a member value
        /// </summary>
        /// <param name="name">Name of the member to set or get</param>
        /// <returns>Value of the member specified</returns>
        public object this[string name]
        {
            get { return Data[name]; }
            set { Data[name] = value; }
        }

        /// <summary>
        /// Sets a member and it's value
        /// </summary>
        /// <param name="name">Name of the member to be assigned</param>
        /// <param name="value">The value to be set</param>
        /// <returns>The value specified</returns>
        public object Set(string name, object value)
        {
            this[name] = value;
            return value;
        }

        /// <summary>
        /// Gets a member's value or null if the specified member is unknown
        /// </summary>
        /// <returns>The value assigned to the member specified, null if member is not set</returns>
        public object Get(string name)
        {
            object value;
            if (!TryGet(name, out value))
            {
                value = null;
            }
            return value;
        }

        /// <summary>
        /// Adds a member and it's value
        /// </summary>
        public void Add(string name, object value)
        {
            Data.Add(name, value);
        }

        /// <summary>
        /// Adds a property
        /// </summary>
        /// <param name="property"></param>
        public void Add(Property property)
        {
            Data.Add(property.Name, property.Value);
        }

        /// <summary>
        /// Removes a member and it's value
        /// </summary>
        /// <returns>True if the member is successfully found and removed; otherwise, false</returns>
        public bool Remove(string name)
        {
            return Data.Remove(name);
        }

        /// <summary>
        /// Gets the value assigned to the specified member
        /// </summary>
        /// <param name="name">The name of the member</param>
        /// <param name="value">When this method returns, contains the value assgned with the specified member, 
        /// if the member is found; null if the member is not found.</param>
        /// <returns>True is the dynamic object contains a member with the specified name; otherwise false</returns>
        public bool TryGet(string name, out object value)
        {
            return Data.TryGetValue(name, out value);
        }

        /// <summary>
        /// Returns a collection of key-value-pairs representing the members and their values hold by this dynamic object.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Property> GetEnumerator()
        {
            return Data.Select(x => new Property(x)).GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Creates an instance of the object represented by this dynamic object.
        /// </summary>
        /// <remarks>Requires the Type property to be set on this dynamic object.</remarks>
        /// <param name="mapper">Optional instance of dynamic object mapper</param>
        public object CreateObject(IDynamicObjectMapper mapper = null)
        {
            return (mapper ?? new DynamicObjectMapper()).Map(this);
        }

        /// <summary>
        /// Creates an instance of the object type specified and populates the object structure represented by this dynamic object.
        /// </summary>
        /// <param name="type">Type of object to be created</param>
        /// <param name="mapper">Optional instance of dynamic object mapper</param>
        public object CreateObject(Type type, IDynamicObjectMapper mapper = null)
        {
            return (mapper ?? new DynamicObjectMapper()).Map(this, type);
        }

        /// <summary>
        /// Creates an instance of the object type specified and populates the object structure represented by this dynamic object.
        /// </summary>
        /// <typeparam name="T">Type of object to be created</typeparam>
        /// <param name="mapper">Optional instance of dynamic object mapper</param>
        public T CreateObject<T>(IDynamicObjectMapper mapper = null)
        {
            return (mapper ?? new DynamicObjectMapper()).Map<T>(this);
        }

        /// <summary>
        /// Creates a dynamic objects representing the object structure defined by the specified object
        /// </summary>
        /// <param name="mapper">Optional instance of dynamic object mapper</param>
        public static DynamicObject CreateDynamicObject(object obj, IDynamicObjectMapper mapper = null)
        {
            return (mapper ?? new DynamicObjectMapper()).MapObject(obj);
        }

        #region IEnumerable<KeyValuePair<string, object>>

        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<string, object>>)Data).GetEnumerator();
        }
        
        #endregion IEnumerable<KeyValuePair<string, object>>

        #region ICollection<KeyValuePair<string, object>>

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            ((ICollection<KeyValuePair<string, object>>)Data).Add(item);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            Data.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return Data.Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, object>>)Data).CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return Data.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return ((ICollection<KeyValuePair<string, object>>)Data).Remove(item);
        }

        #endregion ICollection<KeyValuePair<string, object>>

        #region IDictionary<string, object>

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return Data.ContainsKey(key);
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return Data.Keys; }
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return Data.TryGetValue(key, out value);
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return Data.Values; }
        }

        #endregion IDictionary<string, object>
    }
}
