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

        internal MemberExpression(Expression expression, MemberInfo member)
        {
            Expression = expression;
            Member = member;
        }

        internal MemberExpression(Expression expression, System.Reflection.MemberInfo member)
            : this(expression, MemberInfo.Create(member))
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.Member; } }

        [DataMember(Order = 1, IsRequired = false, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public MemberInfo Member { get; set; }

        public override string ToString()
        {
            return string.Format("{0}->{1}",
                ReferenceEquals(null, Expression) ? null : Expression.ToString(),
                Member);
        }
    }
}
