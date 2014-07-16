// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(MemberAssignment))]
    [KnownType(typeof(MemberListBinding))]
    [KnownType(typeof(MemberMemberBinding))]
    public abstract class MemberBinding
    {
        protected MemberBinding(MemberInfo member)
        {
            Member = member;
        }

        public abstract MemberBindingType BindingType { get; }

        [DataMember]
        public MemberInfo Member { get; private set; }
    }
}
