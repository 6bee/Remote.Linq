// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
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

        internal TypeBinaryExpression(Expression expression, Type type)
        {
            Expression = expression;
            TypeOperand = new TypeInfo(type, false, false);
        }

        public override ExpressionType NodeType => ExpressionType.TypeIs;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo TypeOperand { get; set; }

        public override string ToString()
        {
            return $"{Expression} is {TypeOperand}";
        }
    }
}
