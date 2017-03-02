// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class ParameterExpression : Expression
    {
        public ParameterExpression()
        {
        }

        [Obsolete("Parameter list changed order. This constructor will be removed in a future version.", true)]
        internal ParameterExpression(string parameterName, Type type)
            : this(type, parameterName)
        {
        }

        internal ParameterExpression(TypeInfo parameterType, string parameterName)
        {
            ParameterType = parameterType;
            ParameterName = parameterName;
        }

        internal ParameterExpression(Type parameterType, string parameterName)
            : this(new TypeInfo(parameterType, false, false), parameterName)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Parameter;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public string ParameterName { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ParameterType { get; set; }

        public override string ToString()
        {
            return ParameterName;
        }
    }
}
