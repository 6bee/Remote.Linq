// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ParameterExpression : Expression
    {
        internal ParameterExpression(string parameterName, Type type)
        {
            ParameterName = parameterName;
            ParameterType = new TypeInfo(type);
        }

        public override ExpressionType NodeType { get { return ExpressionType.Parameter; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string ParameterName { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ParameterType { get; private set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", ParameterType, ParameterName);
        }
    }
}
