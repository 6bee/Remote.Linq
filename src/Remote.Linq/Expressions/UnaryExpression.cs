// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class UnaryExpression : Expression
    {
        internal UnaryExpression(Expression operand, UnaryOperator @operator)
        {
            Operand = operand;
            Operator = @operator;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Unary; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public UnaryOperator Operator { get; private set; }

        public override string ToString()
        {
            return string.Format("{1} ({0})", Operand, Operator);
        }
    }
}
