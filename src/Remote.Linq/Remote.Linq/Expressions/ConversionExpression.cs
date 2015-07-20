// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class ConversionExpression : Expression
    {
        public ConversionExpression()
        {
        }

        internal ConversionExpression(Expression operand, Type type)
        {
            Operand = operand;
            Type = new TypeInfo(type, includePropertyInfos: false);
        }

        public override ExpressionType NodeType { get { return ExpressionType.Conversion; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; set; }

        public override string ToString()
        {
            return string.Format("Convert (({0}), {1})", Operand, Type);
        }
    }
}
