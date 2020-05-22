// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class MemberAssignment : MemberBinding
    {
        public MemberAssignment()
        {
        }

        public MemberAssignment(MemberInfo member, Expression expression)
            : base(member)
        {
            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
        }

        public override MemberBindingType BindingType => MemberBindingType.Assignment;

        [DataMember(Order = 2, IsRequired = true)]
        public Expression Expression { get; set; } = null!;
    }
}
