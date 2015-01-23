// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class ParameterExpression : Expression
    {
        public ParameterExpression()
        {
        }

        internal ParameterExpression(string parameterName, Type type)
        {
            ParameterName = parameterName;
            ParameterType = new TypeInfo(type);
        }

        public override ExpressionType NodeType { get { return ExpressionType.Parameter; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public string ParameterName { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ParameterType { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", ParameterType, ParameterName);
        }
    }
}
