// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class ListInitExpression : Expression
    {
        public ListInitExpression()
        {
        }

        public ListInitExpression(NewExpression newExpression, IEnumerable<ElementInit> initializers)
        {
            NewExpression = newExpression ?? throw new ArgumentNullException(nameof(newExpression));
            Initializers = initializers?.ToList() ?? throw new ArgumentNullException(nameof(initializers));
        }

        public override ExpressionType NodeType => ExpressionType.ListInit;

        [DataMember(Order = 1)]
        public NewExpression NewExpression { get; set; } = null!;

        [DataMember(Order = 2)]
        public List<ElementInit> Initializers { get; set; } = null!;

        public override string ToString() => $"{NewExpression} {{ {Initializers.StringJoin(", ")} }}";
    }
}
