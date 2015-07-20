// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;

    [Serializable]
    [DataContract]
    public sealed class CollectionExpression : Expression
    {
        public CollectionExpression()
        {
        }

        internal CollectionExpression(IEnumerable<ConstantExpression> list, Type elementType)
        {
            List = list.ToList();
            ElementType = new TypeInfo(elementType, includePropertyInfos: false);
        }

        public override ExpressionType NodeType { get { return ExpressionType.Collection; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ElementType { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = false)]
        public List<ConstantExpression> List { get; set; }

        public override string ToString()
        {
            return string.Format("{0}", string.Join(",", List.Select(i => string.Format("({0})", i)).ToArray()));
        }
    }
}
