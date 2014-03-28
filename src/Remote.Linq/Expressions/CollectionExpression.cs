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
        internal CollectionExpression(IEnumerable<ConstantExpression> list, Type elementType)
        {
            _elementType = elementType;
            List = list.ToArray();
            ElementTypeName = elementType.FullName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Collection; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ConstantExpression> List { get; private set; }

        [DataMember(Name = "ElementType", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string ElementTypeName { get; private set; }
#else
        private string ElementTypeName { get; set; }
#endif

        public Type ElementType
        {
            get
            {
                if (ReferenceEquals(null, _elementType))
                {
                    try
                    {
                        _elementType = TypeResolver.Instance.ResolveType(ElementTypeName);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Element type '{0}' could not be reconstructed", ElementTypeName));
                    }
                }
                return _elementType;
            }
        }
        [NonSerialized]
        private Type _elementType;

        public override string ToString()
        {
            return string.Format("{0}", string.Join(",", List.Select(i => string.Format("({0})" ,i)).ToArray()));
        }
    }
}
