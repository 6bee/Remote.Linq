// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class UnaryExpression : Expression
    {
        public UnaryExpression()
        {
        }

        public UnaryExpression(UnaryOperator unaryOperator, Expression operand)
        {
            UnaryOperator = unaryOperator;
            Operand = operand;
        }

        public UnaryExpression(UnaryOperator unaryOperator, Expression operand, TypeInfo type, MethodInfo method)
            : this(unaryOperator, operand)
        {
            Type = type;
            Method = method;
        }

        public UnaryExpression(UnaryOperator unaryOperator, Expression operand, Type type, System.Reflection.MethodInfo method)
            : this(unaryOperator, operand, type is null ? null : new TypeInfo(type, false, false), method is null ? null : new MethodInfo(method))
        {
        }

        public override ExpressionType NodeType => ExpressionType.Unary;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
        public UnaryOperator UnaryOperator { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public MethodInfo Method { get; set; }

        public override string ToString()
            => $"{UnaryOperator}({Operand})";
    }
}
