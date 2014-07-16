// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ListInitExpression : Expression
    {
        internal ListInitExpression(NewExpression n, IEnumerable<ElementInit> initializers)
        {
            NewExpression = n;
            Initializers = initializers.ToList().AsReadOnly();
        }

        public override ExpressionType NodeType { get { return ExpressionType.ListInit; } }

        [DataMember]
        public NewExpression NewExpression { get; private set; }

        [DataMember]
        public ReadOnlyCollection<ElementInit> Initializers { get; private set; }
        
        public override string ToString()
        {
            return string.Format("{0} {{ {1} }}", NewExpression, string.Join(", ", Initializers.Select(x => x.ToString()).ToArray()));
        }
    }
}
