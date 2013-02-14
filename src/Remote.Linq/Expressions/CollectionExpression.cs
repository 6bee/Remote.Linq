// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class CollectionExpression : Expression
    {
        internal CollectionExpression(IEnumerable<ConstantValueExpression> list)
        {
            List = list.ToList();
        }

        public override ExpressionType NodeType { get { return ExpressionType.Collection; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ConstantValueExpression> List { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", string.Join(",", List.Select(i => string.Format("({0})" ,i)).ToArray()));
        }
    }
}
