// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ConditionalExpression : Expression
    {
        public ConditionalExpression()
        {
        }

        internal ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
        {
            Test = test;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Conditional; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Test { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression IfTrue { get; set; }

        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public Expression IfFalse { get; set; }

        public override string ToString()
        {
            return string.Format("IF {0} THEN {1} ELSE {2}", Test, IfTrue, IfFalse);
        }
    }
}
