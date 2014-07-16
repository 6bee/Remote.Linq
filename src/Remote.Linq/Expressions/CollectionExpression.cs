// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class CollectionExpression : Expression
    {
        internal CollectionExpression(IEnumerable<ConstantExpression> list, Type elementType)
        {
            List = list.ToList().AsReadOnly();
            ElementType = new TypeInfo(elementType);
        }

        public override ExpressionType NodeType { get { return ExpressionType.Collection; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public ReadOnlyCollection<ConstantExpression> List { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo ElementType { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}", string.Join(",", List.Select(i => string.Format("({0})", i)).ToArray()));
        }
    }
}
