// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "MethodBase")]
    [KnownType(typeof(ConstructorInfo))]
    [KnownType(typeof(MethodInfo))]
    public abstract class MethodBaseInfo : MemberInfo
    {
        protected MethodBaseInfo(System.Reflection.MethodBase methodInfo)
            : base(methodInfo)
        {
            var bindingFlags = methodInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            bindingFlags |= methodInfo.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            BindingFlags = bindingFlags;

            var genericArguments = methodInfo.IsGenericMethod ? methodInfo.GetGenericArguments() : null;
            GenericArgumentTypes = ReferenceEquals(null, genericArguments) || genericArguments.Length == 0
                ? null
                : genericArguments.Select(x => new TypeInfo(x)).ToList().AsReadOnly();

            var parameters = methodInfo.GetParameters();
            ParameterTypes = parameters.Length == 0
                ? null
                : parameters.Select(x => new TypeInfo(x.ParameterType)).ToList().AsReadOnly();
        }

        // TODO: replace binding flags by bool flags
        protected MethodBaseInfo(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes)
            : this(
            name, new TypeInfo(declaringType), bindingFlags, 
            ReferenceEquals(null, genericArguments) ? null : genericArguments.Select(x => new TypeInfo(x)), 
            ReferenceEquals(null, parameterTypes) ? null : parameterTypes.Select(x => new TypeInfo(x)))
        {
        }

        protected MethodBaseInfo(string name, TypeInfo declaringType, BindingFlags bindingFlags, IEnumerable<TypeInfo> genericArguments, IEnumerable<TypeInfo> parameterTypes)
            : base(name, declaringType)
        {
            BindingFlags = bindingFlags;

            GenericArgumentTypes = ReferenceEquals(null, genericArguments) || !genericArguments.Any()
                ? null
                : genericArguments.ToList().AsReadOnly();

            ParameterTypes = ReferenceEquals(null, parameterTypes) || !parameterTypes.Any()
                ? null
                : parameterTypes.ToList().AsReadOnly();
        }

        // TODO: replace binding flags by bool flags
        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public BindingFlags BindingFlags { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public ReadOnlyCollection<TypeInfo> GenericArgumentTypes { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        internal ReadOnlyCollection<TypeInfo> ParameterTypes { get; private set; }

        public bool IsGenericMethod { get { return !ReferenceEquals(null, GenericArgumentTypes) && GenericArgumentTypes.Any(); } }

        public override string ToString()
        {
            var hasGenericArguments = !ReferenceEquals(null, GenericArgumentTypes) && GenericArgumentTypes.Any();
            return string.Format("{0}.{1}{3}{4}{5}({2})",
                DeclaringType,
                Name,
                ReferenceEquals(null, ParameterTypes) ? null : string.Join(", ", ParameterTypes.Select(x => x.ToString()).ToArray()),
                hasGenericArguments ? "<" : null,
                hasGenericArguments ? string.Join(", ", GenericArgumentTypes.Select(x => x.ToString()).ToArray()) : null,
                hasGenericArguments ? ">" : null);
        }
    }
}
