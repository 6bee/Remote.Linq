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
        internal CollectionExpression(IEnumerable<ConstantValueExpression> list, Type elementType)
        {
            List = list.ToList();
            ElementTypeName = elementType.FullName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Collection; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public IEnumerable<ConstantValueExpression> List { get; private set; }

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
                if (ReferenceEquals(_elementType, null))
                {
                    _elementType = Type.GetType(ElementTypeName);
                    if (ReferenceEquals(_elementType, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            _elementType = assembly.GetType(ElementTypeName);
                            if (!ReferenceEquals(_elementType, null)) break;
                        }
                    }
                }
                return _elementType;
            }
        }
        private Type _elementType;

        public override string ToString()
        {
            return string.Format("{0}", string.Join(",", List.Select(i => string.Format("({0})" ,i)).ToArray()));
        }
    }
}
