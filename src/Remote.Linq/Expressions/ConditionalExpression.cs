// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Aqua;
    using Aqua.TypeSystem;
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class ConditionalExpression : Expression
    {
        public ConditionalExpression()
        {
        }

        public ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse, Type type)
            : this(test, ifTrue, ifFalse, ReferenceEquals(null, type) ? null : new TypeInfo(type, false, false))
        {
        }

        public ConditionalExpression(Expression test, Expression ifTrue, Expression ifFalse, TypeInfo type)
        {
            Test = test;
            IfTrue = ifTrue;
            IfFalse = ifFalse;
            Type = type;
        }

        public override ExpressionType NodeType => ExpressionType.Conditional;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Test { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public Expression IfTrue { get; set; }

        [DataMember(Order = 3, IsRequired = true, EmitDefaultValue = false)]
        public Expression IfFalse { get; set; }

        [DataMember(Order = 4, IsRequired = false, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        public override string ToString()
            => $"IF {Test} THEN {IfTrue} ELSE {IfFalse}";
    }
}
