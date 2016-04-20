// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class UnaryExpression : Expression
    {
        public UnaryExpression()
        {
        }

        internal UnaryExpression(Expression operand, UnaryOperator @operator)
        {
            Operand = operand;
            Operator = @operator;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Unary; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
        public UnaryOperator Operator { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; set; }

        public override string ToString()
        {
            return string.Format("{1} ({0})", Operand, Operator);
        }
    }
}
