// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public sealed class ParameterExpression : Expression
    {
        internal ParameterExpression(string parameterName, Type type)
        {
            ParameterName = parameterName;
            ParameterTypeName = type.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Parameter; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string ParameterName { get; private set; }

        [DataMember(Name = "ParameterType", IsRequired = true, EmitDefaultValue = false)]
        private string ParameterTypeName { get; set; }

        public Type ParameterType
        {
            get
            {
                if (ReferenceEquals(_parameterType, null))
                {
                    _parameterType = Type.GetType(ParameterTypeName);
                }
                return _parameterType;
            }
        }
        private Type _parameterType;

        public override string ToString()
        {
            return string.Format("{0}", ParameterName);
        }
    }
}
