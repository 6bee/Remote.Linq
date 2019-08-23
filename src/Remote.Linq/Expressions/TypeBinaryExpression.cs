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
            : this(expression, type is null ? null : new TypeInfo(type, false, false))
        {
        }

        public TypeBinaryExpression(Expression expression, TypeInfo type)
        {
            Expression = expression;
            TypeOperand = type;
        }

        public override ExpressionType NodeType => ExpressionType.TypeIs;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo TypeOperand { get; set; }

        public override string ToString()
            => $"{Expression} is {TypeOperand}";
    }
}
