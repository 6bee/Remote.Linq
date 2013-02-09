// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public sealed class BinaryExpression : Expression
    {
        internal BinaryExpression(Expression leftOperand, Expression rightOperand, BinaryOperator @operator)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operator = @operator;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Binary; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression LeftOperand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression RightOperand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public BinaryOperator Operator { get; private set; }

        public override string ToString()
        {
            return string.Format("({0}) {1} ({2})", LeftOperand, Operator, RightOperand);
        }
    }
}
