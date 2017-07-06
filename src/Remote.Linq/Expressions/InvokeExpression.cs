// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class InvokeExpression : Expression
    {
        public InvokeExpression()
        {
        }

        public InvokeExpression(Expression expression, IEnumerable<Expression> arguments)
        {
            Expression = expression;
            Arguments = arguments?.Any() ?? false ? arguments.ToList() : null;
        }

        public override ExpressionType NodeType => ExpressionType.Invoke;

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public Expression Expression { get; set; }

        [DataMember(Order = 2, IsRequired = false, EmitDefaultValue = false)]
        public List<Expression> Arguments { get; set; }

        public override string ToString()
            => $".Invoke {Expression}";
    }
}
