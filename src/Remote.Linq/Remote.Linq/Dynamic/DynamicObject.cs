// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Dynamic
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract(IsReference = true)]
    [DebuggerDisplay("Count = {MemberCount}")]
    public partial class DynamicObject : IEnumerable<DynamicObject.Property>, IDictionary<string, object>
    {
        [Serializable]
        [DataContract(Name = "DynamicObjectProperty")]
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

        /// <summary>
        /// Creates a new instance of a dynamic object
        /// </summary>
        public DynamicObject()
            : this(default(TypeInfo))
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
            Members = new List<Property>();
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

            Members = members.Select(x => new Property(x)).ToList();
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

            Members = members.ToList();
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
            Members = dynamicObject.Members;
        }

        /// <summary>
        /// Gets or sets the type of object represented by this dynamic object instance
        /// </summary>
        [DataMember(Order = 1)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 2)]
        public List<Property> Members { get; set; }

        /// <summary>
        /// Gets the count of members (dynamically added properties) hold by this dynamic object
        /// </summary>
        public int MemberCount
        {
            get { return Members.Count; }
        }

        /// <summary>
        /// Gets a collection of member names hold by this dynamic object
        /// </summary>
        public IEnumerable<string> MemberNames
        {
            get { return Members.Select(x => x.Name).ToList(); }
        }

        /// <summary>
        /// Gets a collection of member values hold by this dynamic object
        /// </summary>
        public IEnumerable<object> Values
        {
            get { return Members.Select(x => x.Value).ToList(); }
        }

        /// <summary>
        /// Gets or sets a member value
        /// </summary>
        /// <param name="name">Name of the member to set or get</param>
        /// <returns>Value of the member specified</returns>
        public object this[string name]
        {
            get
            {
                object value;
                if (TryGet(name, out value))
                {
                    return value;
                }

                throw new Exception(string.Format("Member not found for name '{0}'", name));
            }
            set
            {
                Set(name, value);
            }
        }

        /// <summary>
        /// Sets a member and it's value
        /// </summary>
        /// <param name="name">Name of the member to be assigned</param>
        /// <param name="value">The value to be set</param>
        /// <returns>The value specified</returns>
        public object Set(string name, object value)
        {
            var property = Members.SingleOrDefault(x => string.Equals(x.Name, name));

            if (ReferenceEquals(null, property))
            {
                property = new Property(name, value);
                Members.Add(property);
            }
            else
            {
                property.Value = value;
            }

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
            Add(new Property(name, value));
        }

        /// <summary>
        /// Adds a property
        /// </summary>
        public void Add(Property property)
        {
            Members.Add(property);
        }

        /// <summary>
        /// Removes a member and it's value
        /// </summary>
        /// <returns>True if the member is successfully found and removed; otherwise, false</returns>
        public bool Remove(string name)
        {
            var property = Members.SingleOrDefault(x => string.Equals(x.Name, name));

            if (ReferenceEquals(null, property))
            {
                return false;
            }

            Members.Remove(property);
            return true;
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
            var property = Members.SingleOrDefault(x => string.Equals(x.Name, name));

            if (ReferenceEquals(null, property))
            {
                value = null;
                return false;
            }

            value = property.Value;
            return true;
        }

        /// <summary>
        /// Returns a collection of key-value-pairs representing the members and their values hold by this dynamic object.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Property> GetEnumerator()
        {
            return Members.GetEnumerator();
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
            foreach (var member in Members)
            {
                yield return (KeyValuePair<string, object>)member;
            }
        }

        #endregion IEnumerable<KeyValuePair<string, object>>

        #region ICollection<KeyValuePair<string, object>>

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            Add(new Property(item));
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            Members.Clear();
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return Members.Any(x => string.Equals(x.Name, item.Key) && object.Equals(x.Value, item.Value));
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            ((IEnumerable<KeyValuePair<string, object>>)this).ToList().CopyTo(array, arrayIndex);
        }

        int ICollection<KeyValuePair<string, object>>.Count
        {
            get { return Members.Count; }
        }

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly
        {
            get { return false; }
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return Remove(item.Key);
        }

        #endregion ICollection<KeyValuePair<string, object>>

        #region IDictionary<string, object>

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return Members.Any(x => string.Equals(x.Name, key));
        }

        ICollection<string> IDictionary<string, object>.Keys
        {
            get { return MemberNames.ToList(); }
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return TryGet(key, out value);
        }

        ICollection<object> IDictionary<string, object>.Values
        {
            get { return Values.ToList(); }
        }

        #endregion IDictionary<string, object>
    }
}
