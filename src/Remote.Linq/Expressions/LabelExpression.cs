// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class LabelExpression : Expression
    {
        public LabelExpression()
        {
        }

        public LabelExpression(LabelTarget target, Expression defaultValue)
        {
            Target = target;
            DefaultValue = defaultValue;
        }

        public override ExpressionType NodeType => ExpressionType.Label;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public LabelTarget Target { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public Expression DefaultValue { get; set; }

        public override string ToString()
            => $".Label {Target} {DefaultValue}";
    }
}
