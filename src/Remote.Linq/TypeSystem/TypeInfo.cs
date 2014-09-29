// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "Type")]
    public sealed class TypeInfo
    {
        public TypeInfo(Type type)
        {
            if (ReferenceEquals(null, type)) throw new ArgumentNullException("type");
            _type = type;
            Name = type.Name;
            Namespace = type.Namespace;
            if (type.IsNested && !type.IsGenericParameter)
            {
                DeclaringType = new TypeInfo(type.DeclaringType);
            }
            if (type.GetIsGenericType())
            {
                GenericArguments = type.GetGenericArguments().Select(x => new TypeInfo(x)).ToList().AsReadOnly();
            }
            IsAnonymousType = type.IsAnonymousType();
            if (IsAnonymousType)
            {
                Properties = type.GetProperties().Select(x => x.Name).ToList().AsReadOnly();
            }
        }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string Name { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public string Namespace { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo DeclaringType { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReadOnlyCollection<TypeInfo> GenericArguments { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        internal bool IsAnonymousType { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        internal ReadOnlyCollection<string> Properties { get; private set; }

        public bool IsNested { get { return !ReferenceEquals(null, DeclaringType); } }
        public bool IsGenericType { get { return !ReferenceEquals(null, GenericArguments) && GenericArguments.Any(); } }

        internal string FullName
        {
            get
            {
                if (IsNested)
                {
                    var fullname = string.Format("{0}+{1}",
                        DeclaringType.FullName,
                        Name);
                    return fullname;
                }
                else
                {
                    var fullname = string.Format("{0}{1}{2}",
                        Namespace,
                        string.IsNullOrEmpty(Namespace) ? null : ".",
                        Name);
                    return fullname;
                }
            }
        }

        /// <summary>
        /// Resolves this type info instance to it's type using the default type resolver instance.
        /// </summary>
        public Type Type
        {
            get
            {
                if (ReferenceEquals(null, _type))
                {
                    _type = TypeResolver.Instance.ResolveType(this);
                }
                return _type;
            }
        }
        [NonSerialized]
        private Type _type;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as TypeInfo);
        }

        public bool Equals(TypeInfo typeInfo)
        {
            if (ReferenceEquals(null, typeInfo)) return false;
            if (ReferenceEquals(this, typeInfo)) return true;
            var s0 = ToString();
            var s1 = typeInfo.ToString();
            return string.Compare(s0, s1) == 0;
        }

        public override int GetHashCode()
        {
            var s = ToString();
            return s.GetHashCode();
        }

        public override string ToString()
        {
            var genericArguments = IsGenericType
                ? string.Format("[{0}]", string.Join(",", GenericArguments.Select(x => x.ToString()).ToArray()))
                : null;
            return string.Format("{0}{1}", FullName, genericArguments);
        }

        public static implicit operator Type(TypeInfo t)
        {
            return t.Type;
        }
    }
}
