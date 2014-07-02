// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ConditionalExpression : Expression
    {
        internal ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse)
        {
            Test = test;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Conditional; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Test { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression IfTrue { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression IfFalse { get; private set; }

        public override string ToString()
        {
            return string.Format("IF {0} THEN {1} ELSE {2}", Test, IfTrue, IfFalse);
        }
    }
}
