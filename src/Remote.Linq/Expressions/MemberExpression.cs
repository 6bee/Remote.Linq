// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class MemberExpression : Expression
    {
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

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Expression Expression { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public MemberInfo Member { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}->{1}", 
                ReferenceEquals(null, Expression) ? null : Expression.ToString(), 
                Member);
        }
    }
}
