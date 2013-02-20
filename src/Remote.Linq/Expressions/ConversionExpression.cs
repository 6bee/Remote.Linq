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
                if (ReferenceEquals(_type, null))
                {
                    _type = Type.GetType(TypeName);
                    if (ReferenceEquals(_type, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            _type = assembly.GetType(TypeName);
                            if (!ReferenceEquals(_type, null)) break;
                        }
                        if (ReferenceEquals(_type, null))
                        {
                            throw new Exception(string.Format("Type '{0}' could not be reconstructed", TypeName));
                        }
                    }
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
