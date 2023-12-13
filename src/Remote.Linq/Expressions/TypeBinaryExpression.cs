// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class TypeBinaryExpression : Expression
    {
        public TypeBinaryExpression()
        {
        }

        public TypeBinaryExpression(Expression expression, Type type)
            : this(expression, type.AsTypeInfo())
        {
        }

        public TypeBinaryExpression(Expression expression, TypeInfo type)
        {
            Expression = expression.CheckNotNull();
            TypeOperand = type.CheckNotNull();
        }

        public override ExpressionType NodeType => ExpressionType.TypeIs;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; } = null!;

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo TypeOperand { get; set; } = null!;

        public override string ToString() => $"{Expression} is {TypeOperand}";
    }
}