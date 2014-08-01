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

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression LeftOperand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression RightOperand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public BinaryOperator Operator { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public bool IsLiftedToNull { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public MethodInfo Method { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public LambdaExpression Conversion { get; private set; }

        public override string ToString()
        {
            return string.Format("({0}) {1} ({2})", LeftOperand, Operator, RightOperand);
        }
    }
}
