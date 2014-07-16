// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ConversionExpression : Expression
    {
        internal ConversionExpression(Expression operand, Type type)
        {
            Operand = operand;
            Type = new TypeInfo(type);
        }

        public override ExpressionType NodeType { get { return ExpressionType.Conversion; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; private set; }

        public override string ToString()
        {
            return string.Format("Convert (({0}), {1})", Operand, Type);
        }
    }
}
