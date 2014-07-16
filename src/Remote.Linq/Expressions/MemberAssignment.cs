// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class MemberAssignment : MemberBinding
    {
        public MemberAssignment(MemberInfo member, Expression expression)
            : base(member)
        {
            Expression = expression;
        }

        public override MemberBindingType BindingType { get { return MemberBindingType.Assignment; } }

        [DataMember]
        public Expression Expression { get; private set; }
    }
}
