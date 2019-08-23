// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class MemberExpression : Expression
    {
        public MemberExpression()
        {
        }

        public MemberExpression(Expression expression, MemberInfo member)
        {
            Expression = expression;
            Member = member;
        }

        public MemberExpression(Expression expression, System.Reflection.MemberInfo member)
            : this(expression, MemberInfo.Create(member))
        {
        }

        public override ExpressionType NodeType => ExpressionType.MemberAccess;

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public MemberInfo Member { get; set; }

        public override string ToString()
            => $"{Expression}.{Member?.Name}";
    }
}
