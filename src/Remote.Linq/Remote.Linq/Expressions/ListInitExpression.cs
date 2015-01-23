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

        internal ListInitExpression(NewExpression n, IEnumerable<ElementInit> initializers)
        {
            NewExpression = n;
            Initializers = initializers.ToList();
        }

        public override ExpressionType NodeType { get { return ExpressionType.ListInit; } }

        [DataMember(Order = 1)]
        public NewExpression NewExpression { get; set; }

        [DataMember(Order = 2)]
        public List<ElementInit> Initializers { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {{ {1} }}", NewExpression, string.Join(", ", Initializers.Select(x => x.ToString()).ToArray()));
        }
    }
}
