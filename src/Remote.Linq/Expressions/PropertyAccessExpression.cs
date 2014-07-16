// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using Remote.Linq.TypeSystem;
using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class PropertyAccessExpression : Expression
    {
        internal PropertyAccessExpression(Expression instance, PropertyInfo propertyInfo)
        {
            Instance = instance;
            Property = propertyInfo;
        }

        internal PropertyAccessExpression(Expression instance, System.Reflection.PropertyInfo propertyInfo)
            : this(instance, new PropertyInfo(propertyInfo))
        {
        }

        internal PropertyAccessExpression(Expression instance, string propertyName, Type propertyType, Type declaringType)
            : this(instance, new PropertyInfo(propertyName, propertyType, declaringType))
        {
        }

        public override ExpressionType NodeType { get { return ExpressionType.PropertyAccess; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public PropertyInfo Property { get; private set; }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Expression Instance { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", ReferenceEquals(null, Instance) ? Property.DeclaringType.ToString() : Instance.ToString(), Property.Name);
        }
    }
}
