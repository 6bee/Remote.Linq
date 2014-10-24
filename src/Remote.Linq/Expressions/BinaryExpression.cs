// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class BinaryExpression : Expression
    {
        public BinaryExpression()
        {
        }

        internal BinaryExpression(Expression leftOperand, Expression rightOperand, BinaryOperator @operator)
        {
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
            Operator = @operator;
        }

        internal BinaryExpression(Expression leftOperand, Expression rightOperand, BinaryOperator @operator, bool liftToNull, System.Reflection.MethodInfo method, LambdaExpression conversion = null)
            : this(leftOperand, rightOperand, @operator)
        {
            IsLiftedToNull = liftToNull;
            Method = ReferenceEquals(null, method) ? null : new MethodInfo(method);
            Conversion = conversion;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Binary; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression LeftOperand { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression RightOperand { get; set; }

        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = true)]
        public BinaryOperator Operator { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public bool IsLiftedToNull { get; set; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public MethodInfo Method { get; set; }

        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public LambdaExpression Conversion { get; set; }

        public override string ToString()
        {
            return string.Format("({0}) {1} ({2})", LeftOperand, Operator, RightOperand);
        }
    }
}
