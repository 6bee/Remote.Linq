// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [DataContract]
    [KnownType(typeof(MemberAssignment)), XmlInclude(typeof(MemberAssignment))]
    [KnownType(typeof(MemberListBinding)), XmlInclude(typeof(MemberListBinding))]
    [KnownType(typeof(MemberMemberBinding)), XmlInclude(typeof(MemberMemberBinding))]
    public abstract class MemberBinding
    {
        protected MemberBinding(MemberInfo member)
        {
            Member = member;
        }

        public abstract MemberBindingType BindingType { get; }

        [DataMember(Order = 1)]
        public MemberInfo Member { get; set; }
    }
}
