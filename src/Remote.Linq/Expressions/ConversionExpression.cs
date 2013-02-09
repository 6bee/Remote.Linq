// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public sealed class ConversionExpression : Expression
    {
        internal ConversionExpression(Expression operand, Type type)
        {
            Operand = operand;
            TypeName = type.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Conversion; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; private set; }

        [DataMember(Name = "Type", IsRequired = true, EmitDefaultValue = false)]
        private string TypeName { get; set; }

        public Type Type
        {
            get
            {
                if (ReferenceEquals(_type, null))
                {
                    _type = Type.GetType(TypeName);
                }
                return _type;
            }
        }
        private Type _type;

        public override string ToString()
        {
            return string.Format("Convert (({0}), {1})", Operand, Type.FullName);
        }
    }
}
