// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class LoopExpression : Expression
    {
        public LoopExpression()
        {
        }

        public LoopExpression(Expression body, LabelTarget breakLabel, LabelTarget continueLabel)
        {
            Body = body;
            BreakLabel = breakLabel;
            ContinueLabel = continueLabel;
        }

        public override ExpressionType NodeType => ExpressionType.Loop;

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public Expression Body { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public LabelTarget BreakLabel { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public LabelTarget ContinueLabel { get; set; }

        public override string ToString()
            => $"loop(body:{Body}, break:{BreakLabel}, continue:{ContinueLabel})";
    }
}
