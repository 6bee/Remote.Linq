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
    [KnownType(typeof(object))]
    [KnownType(typeof(object[]))]
    [DebuggerDisplay("Count = {MemberCount}")]
    public partial class DynamicObject : IEnumerable<KeyValuePair<string, object>>
    {
        /// <summary>
        /// Creates a new instance of a dynamic object
        /// </summary>
        public DynamicObject()
        {
            Members = new Dictionary<string, object>();
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
            : this()
        {
            Type = type;
        }

        /// <summary>
        /// Creates a new instance of a dynamic object, setting the specified members
        /// </summary>
        /// <param name="members">Initial collection of properties and values</param>
        /// <exception cref="ArgumentNullException">The specified members collection is null</exception>
        public DynamicObject(IEnumerable<KeyValuePair<string, object>> members)
            : this()
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
        /// Creates a new instance of a dynamic object, representing the object structure defined by the specified object
        /// </summary>
        /// <param name="obj">The object to be represented by the new dynamic object</param>
        /// <exception cref="ArgumentNullException">The specified object is null</exception>
        public DynamicObject(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                throw new ArgumentNullException("obj");
            }

            var dynamicObject = DynamicObjectMapper.InstanceProvider().MapObject(obj);
            Type = dynamicObject.Type;
            Members = dynamicObject.Members;
        }

        /// <summary>
        /// Gets or sets the type of object represented by this dynamic object instance
        /// </summary>
        [DataMember]
        public TypeInfo Type { get; set; }

        [DataMember]
        private Dictionary<string, object> Members { get; set; }

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
            get { return Members.Keys.ToList(); }
        }

        /// <summary>
        /// Gets a collection of member values hold by this dynamic object
        /// </summary>
        public IEnumerable<object> Values
        {
            get { return Members.Values.ToList(); }
        }

        /// <summary>
        /// Gets or sets a member value
        /// </summary>
        /// <param name="name">Name of the member to set or get</param>
        /// <returns>Value of the member specified</returns>
        public object this[string name]
        {
            get { return Members[name]; }
            set { Members[name] = value; }
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
            Members.Add(name, value);
        }

        /// <summary>
        /// Removes a member and it's value
        /// </summary>
        /// <returns>True if the member is successfully found and removed; otherwise, false</returns>
        public bool Remove(string name)
        {
            return Members.Remove(name);
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
            return Members.TryGetValue(name, out value);
        }

        /// <summary>
        /// Returns a collection of key-value-pairs representing the members and their values hold by this dynamic object.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
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
        public object CreateObject()
        {
            return DynamicObjectMapper.InstanceProvider().Map(this);
        }

        /// <summary>
        /// Creates an instance of the object type specified and populates the object structure represented by this dynamic object.
        /// </summary>
        /// <param name="type">Type of object to be created</param>
        public object CreateObject(Type type)
        {
            return DynamicObjectMapper.InstanceProvider().Map(this, type);
        }

        /// <summary>
        /// Creates an instance of the object type specified and populates the object structure represented by this dynamic object.
        /// </summary>
        /// <typeparam name="T">Type of object to be created</typeparam>
        public T CreateObject<T>()
        {
            return DynamicObjectMapper.InstanceProvider().Map<T>(this);
        }

        /// <summary>
        /// Creates a dynamic objects representing the object structure defined by the specified object
        /// </summary>
        public static DynamicObject CreateDynamicObject(object obj)
        {
            return DynamicObjectMapper.InstanceProvider().MapObject(obj);
        }
    }
}
