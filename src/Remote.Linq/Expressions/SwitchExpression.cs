// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class SwitchExpression : Expression
    {
        public SwitchExpression()
        {
        }

        public SwitchExpression(Expression switchValue, MethodInfo comparison, Expression defaultExpression, List<SwitchCase> cases)
        {
            SwitchValue = switchValue;
            Comparison = comparison;
            DefaultExpression = defaultExpression;
            Cases = cases;
        }

        public SwitchExpression(Expression switchValue, System.Reflection.MethodInfo comparison, Expression defaultExpression, List<SwitchCase> cases)
            : this(switchValue, comparison is null ? null : new MethodInfo(comparison), defaultExpression, cases)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Switch;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression SwitchValue { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public MethodInfo Comparison { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public Expression DefaultExpression { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public List<SwitchCase> Cases { get; set; }
    }
}
