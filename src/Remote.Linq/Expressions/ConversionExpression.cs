// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

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
            _type = type;
            Operand = operand;
            TypeName = type.FullName;//.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Conversion; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public Expression Operand { get; private set; }

        [DataMember(Name = "Type", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string TypeName { get; private set; }
#else
        private string TypeName { get; set; }
#endif

        public Type Type
        {
            get
            {
                if (ReferenceEquals(null, _type))
                {
                    _type = TypeResolver.Instance.ResolveType(TypeName);
                }
                return _type;
            }
        }
        [NonSerialized]
        private Type _type;

        public override string ToString()
        {
            return string.Format("Convert (({0}), {1})", Operand, Type.FullName);
        }
    }
}
