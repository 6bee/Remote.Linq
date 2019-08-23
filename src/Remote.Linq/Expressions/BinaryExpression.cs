// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class BinaryExpression : Expression
    {
        public BinaryExpression()
        {
        }

        public BinaryExpression(BinaryOperator binaryOperator, Expression leftOperand, Expression rightOperand)
        {
            BinaryOperator = binaryOperator;
            LeftOperand = leftOperand;
            RightOperand = rightOperand;
        }

        public BinaryExpression(BinaryOperator binaryOperator, Expression leftOperand, Expression rightOperand, bool liftToNull, MethodInfo method, LambdaExpression conversion = null)
            : this(binaryOperator, leftOperand, rightOperand)
        {
            IsLiftedToNull = liftToNull;
            Method = method;
            Conversion = conversion;
        }

        public BinaryExpression(BinaryOperator binaryOperator, Expression leftOperand, Expression rightOperand, bool liftToNull, System.Reflection.MethodInfo method, LambdaExpression conversion = null)
            : this(binaryOperator, leftOperand, rightOperand, liftToNull, method is null ? null : new MethodInfo(method), conversion)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Binary;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
        public BinaryOperator BinaryOperator { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression LeftOperand { get; set; }

        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public Expression RightOperand { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public bool IsLiftedToNull { get; set; }

        [DataMember(Order = 5, IsRequired = false, EmitDefaultValue = false)]
        public MethodInfo Method { get; set; }

        [DataMember(Order = 6, IsRequired = false, EmitDefaultValue = false)]
        public LambdaExpression Conversion { get; set; }

        public override string ToString()
            => $"({LeftOperand} {BinaryOperator} {RightOperand})";
    }
}
