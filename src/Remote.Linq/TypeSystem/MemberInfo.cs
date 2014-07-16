// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "Member")]
    [KnownType(typeof(ConstructorInfo))]
    [KnownType(typeof(FieldInfo))]
    [KnownType(typeof(MethodInfo))]
    [KnownType(typeof(PropertyInfo))]
    public abstract class MemberInfo
    {
        protected MemberInfo(System.Reflection.MemberInfo memberInfo)
        {
            if (ReferenceEquals(null, memberInfo)) throw new ArgumentNullException("memberInfo");

            Name = memberInfo.Name;
            DeclaringType = new TypeInfo(memberInfo.DeclaringType);
        }

        protected MemberInfo(string name, Type declaringType)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException("name", "Expected a valid member name");
            if (ReferenceEquals(null, declaringType)) throw new ArgumentNullException("declaringType");

            Name = name;
            DeclaringType = new TypeInfo(declaringType);
        }

        public abstract MemberTypes MemberType { get; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string Name { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo DeclaringType { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", DeclaringType, Name);
        }

        internal static MemberInfo Create(System.Reflection.MemberInfo memberInfo)
        {
            switch (memberInfo.GetMemberType())
            {
                case MemberTypes.Field:
                    return new FieldInfo((System.Reflection.FieldInfo)memberInfo);

                case MemberTypes.Constructor:
                    return new ConstructorInfo((System.Reflection.ConstructorInfo)memberInfo);

                case MemberTypes.Property:
                    return new PropertyInfo((System.Reflection.PropertyInfo)memberInfo);

                case MemberTypes.Method:
                    return new MethodInfo((System.Reflection.MethodInfo)memberInfo);

                default:
                    throw new Exception(string.Format("Not supported member type: {0}", memberInfo.GetMemberType()));
            }
        }

        public static implicit operator System.Reflection.MemberInfo(MemberInfo memberInfo)
        {
            switch (memberInfo.MemberType)
            {
                case MemberTypes.Field:
                    return (System.Reflection.FieldInfo)(FieldInfo)memberInfo;

                case MemberTypes.Constructor:
                    return (System.Reflection.ConstructorInfo)(ConstructorInfo)memberInfo;

                case MemberTypes.Property:
                    return (System.Reflection.PropertyInfo)(PropertyInfo)memberInfo;

                case MemberTypes.Method:
                    return (System.Reflection.MethodInfo)(MethodInfo)memberInfo;

                default:
                    throw new NotImplementedException(string.Format("Implementation missing for conversion of member type: {0}", memberInfo.MemberType));
            }
        }
    }
}
