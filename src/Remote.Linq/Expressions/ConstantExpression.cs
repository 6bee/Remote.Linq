// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ConstantExpression : Expression
    {
        internal ConstantExpression(object value, Type type = null)
        {
            Value = value;
            if (ReferenceEquals(null, type))
            {
                if (ReferenceEquals(null, value))
                {
                    _type = typeof(object);
                }
                else
                {
                    _type = value.GetType();
                }
            }
            else
            {
                _type = type;
            }
            TypeName = _type.FullName;//.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Constant; } }

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public object Value { get; private set; }

        [DataMember(Name = "Type", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal string TypeName { get; private set; }
#else
        private string TypeName { get; set; }
#endif

        public Type Type
        {
            get
            {
                if (ReferenceEquals(null, _type))
                {
                    _type = TypeResolver.Instance.ResolveType(TypeName);
                }
                return _type;
            }
        }
        [NonSerialized]
        private Type _type;

        public override string ToString()
        {
            return string.Format("{1}{0}{1}", Value ?? "null", Value is string || Value is char ? "'" : null);
        }
    }
}
