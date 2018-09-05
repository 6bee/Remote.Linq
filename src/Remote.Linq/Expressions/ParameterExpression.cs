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

        public ParameterExpression(TypeInfo parameterType, string parameterName, int instanceId)
        {
            ParameterType = parameterType;
            ParameterName = parameterName;
            InstanceId = instanceId;
        }

        public ParameterExpression(Type parameterType, string parameterName, int instanceId)
            : this(new TypeInfo(parameterType, false, false), parameterName, instanceId)
        {
        }

        public override ExpressionType NodeType => ExpressionType.Parameter;

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public string ParameterName { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ParameterType { get; set; }

        /// <summary>
        /// Gets or sets an instance id,
        /// which is used to denote a specific instance of <see cref="System.Linq.Expressions.ParameterExpression"/> within an expression tree.
        /// </summary>
        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public int InstanceId { get; set; }

        public override string ToString()
            => ParameterName ?? "_";
    }
}
