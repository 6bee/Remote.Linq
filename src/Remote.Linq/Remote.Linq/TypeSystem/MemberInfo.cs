// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [DataContract(Name = "Member", IsReference = true)]
    [KnownType(typeof(ConstructorInfo)), XmlInclude(typeof(ConstructorInfo))]
    [KnownType(typeof(FieldInfo)), XmlInclude(typeof(FieldInfo))]
    [KnownType(typeof(MethodInfo)), XmlInclude(typeof(MethodInfo))]
    [KnownType(typeof(PropertyInfo)), XmlInclude(typeof(PropertyInfo))]
    public abstract class MemberInfo
    {
        protected MemberInfo()
        {
        }

        protected MemberInfo(System.Reflection.MemberInfo memberInfo, Dictionary<Type, TypeInfo> referenceTracker)
        {
            if (ReferenceEquals(null, memberInfo))
            {
                throw new ArgumentNullException("memberInfo");
            }

            Name = memberInfo.Name;
            DeclaringType = TypeInfo.Create(referenceTracker, memberInfo.DeclaringType, includePropertyInfos: false);
        }

        protected MemberInfo(string name, TypeInfo declaringType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name", "Expected a valid member name");
            }

            if (ReferenceEquals(null, declaringType))
            {
                throw new ArgumentNullException("declaringType");
            }

            Name = name;
            DeclaringType = declaringType;
        }

        public abstract MemberTypes MemberType { get; }

        [DataMember(Order = 1, EmitDefaultValue = false)]
        public string Name { get; set; }

        [DataMember(Order = 2, EmitDefaultValue = false)]
        public TypeInfo DeclaringType { get; set; }

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

        internal System.Reflection.MemberInfo ResolveMemberInfo(ITypeResolver typeResolver)
        {
            switch (MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)this).ResolveField(typeResolver);

                case MemberTypes.Constructor:
                    return ((ConstructorInfo)this).ResolveConstructor(typeResolver);

                case MemberTypes.Property:
                    return ((PropertyInfo)this).ResolveProperty(typeResolver);

                case MemberTypes.Method:
                    return ((MethodInfo)this).ResolveMethod(typeResolver);

                default:
                    throw new NotImplementedException(string.Format("Implementation missing for conversion of member type: {0}", this.MemberType));
            }
        }

        public static explicit operator System.Reflection.MemberInfo(MemberInfo memberInfo)
        {
            return memberInfo.ResolveMemberInfo(TypeResolver.Instance);
        }
    }
}
