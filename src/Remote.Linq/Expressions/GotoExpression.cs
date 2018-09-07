// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class GotoExpression : Expression
    {
        public GotoExpression()
        {
        }

        public GotoExpression(GotoExpressionKind kind, LabelTarget target, Type type, Expression value)
            : this(kind, target, type is null ? null : new TypeInfo(type, false, false), value)
        {
        }

        public GotoExpression(GotoExpressionKind kind, LabelTarget target, TypeInfo type, Expression value)
        {
            Kind = kind;
            Target = target;
            Type = type;
            Value = value;
        }

        public override ExpressionType NodeType => ExpressionType.Goto;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = true)]
        public GotoExpressionKind Kind { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public LabelTarget Target { get; set; }

        [DataMember(Order = 3, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public Expression Value { get; set; }

        public override string ToString()
            => $".Goto {Kind} {Target} {Value}";
    }
}
