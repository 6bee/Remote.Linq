// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Expressions
{
    using Remote.Linq.TypeSystem;
    using System;
    using System.Runtime.Serialization;
    using System.Xml.Serialization;

    [Serializable]
    [DataContract]
    [KnownType(typeof(Remote.Linq.DynamicQuery.QueryableResourceDescriptor)), XmlInclude(typeof(Remote.Linq.DynamicQuery.QueryableResourceDescriptor))]
    [KnownType(typeof(Remote.Linq.VariableQueryArgument)), XmlInclude(typeof(Remote.Linq.VariableQueryArgument))]
    [KnownType(typeof(Remote.Linq.VariableQueryArgumentList)), XmlInclude(typeof(Remote.Linq.VariableQueryArgumentList))]
    public sealed class ConstantExpression : Expression
    {
        public ConstantExpression()
        {
        }

        internal ConstantExpression(object value, Type type = null)
        {
            if (ReferenceEquals(null, type))
            {
                if (ReferenceEquals(null, value))
                {
                    type = typeof(object);
                }
                else
                {
                    type = value.GetType();
                }
            }

            Type = new TypeInfo(type, includePropertyInfos: false);
            Value = value;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Constant; } }

        [DataMember(Order = 1, IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo Type { get; set; }

        [DataMember(Order = 2, IsRequired = true, EmitDefaultValue = true)]
        public object Value { get; set; }

        public override string ToString()
        {
            return string.Format("{1}{0}{1}", Value ?? "null", Value is string || Value is char ? "'" : null);
        }
    }
}
