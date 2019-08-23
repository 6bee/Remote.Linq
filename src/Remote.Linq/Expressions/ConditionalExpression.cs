// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class ConditionalExpression : Expression
    {
        public ConditionalExpression()
        {
        }

        public ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
        {
            Test = test;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public override ExpressionType NodeType => ExpressionType.Conditional;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Test { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression IfTrue { get; set; }

        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public Expression IfFalse { get; set; }

        public override string ToString()
            => $"IF {Test} THEN {IfTrue} ELSE {IfFalse}";
    }
}
