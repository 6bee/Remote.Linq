// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
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

        internal MemberInitExpression(NewExpression newExpression, IEnumerable<MemberBinding> bindings)
        {
            NewExpression = newExpression;
            Bindings = bindings.ToList();
        }

        public override ExpressionType NodeType { get { return ExpressionType.MemberInit; } }

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public NewExpression NewExpression { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<MemberBinding> Bindings { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", NewExpression, string.Join(", ", Bindings.Select(b => b.ToString()).ToArray()));
        }
    }
}
