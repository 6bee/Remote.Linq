// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class DefaultExpression : Expression
    {
        public DefaultExpression()
        {
        }

        public DefaultExpression(Type type)
            : this(type is null ? null : new TypeInfo(type, false, false))
        {
        }

        public DefaultExpression(TypeInfo type)
        {
            Type = type;
        }

        public override ExpressionType NodeType => ExpressionType.Default;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        public override string ToString()
            => $".Default {Type}";
    }
}
