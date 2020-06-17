// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.Extensions;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class MemberInitExpression : Expression
    {
        public MemberInitExpression()
        {
        }

        public MemberInitExpression(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            NewExpression = newExpression.CheckNotNull(nameof(newExpression));
            Bindings = bindings.CheckNotNull(nameof(bindings)).ToList();
        }

        public override ExpressionType NodeType => ExpressionType.MemberInit;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public NewExpression NewExpression { get; set; } = null!;

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<MemberBinding> Bindings { get; set; } = null!;

        public override string ToString() => $"{NewExpression} {Bindings.StringJoin(", ")}";
    }
}
