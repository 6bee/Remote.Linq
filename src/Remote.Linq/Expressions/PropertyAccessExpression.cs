// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public sealed class PropertyAccessExpression : Expression
    {
        internal PropertyAccessExpression(PropertyInfo propertyInfo, PropertyAccessExpression parent = null)
            : this(propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.DeclaringType, parent)
        {
        }

        internal PropertyAccessExpression(string propertyName, Type propertyType, Type declaringType, PropertyAccessExpression parent = null)
        {
            PropertyName = propertyName;
            PropertyType = propertyType.AssemblyQualifiedName;
            DeclaringType = declaringType.AssemblyQualifiedName;
            Parent = parent;
        }

        public override ExpressionType NodeType { get { return ExpressionType.PropertyAccess; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        private string PropertyName { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        private string PropertyType { get; set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        private string DeclaringType { get; set; }

        public PropertyInfo PropertyInfo
        {
            get
            {
                if (ReferenceEquals(_propertyInfo, null))
                {
                    var declaringType = Type.GetType(DeclaringType);
                    var propertyType = Type.GetType(PropertyType);
                    var propertyInfo = declaringType.GetProperty(PropertyName, propertyType);
                    if (propertyInfo == null) propertyInfo = declaringType.GetProperty(PropertyName);
                    _propertyInfo = propertyInfo;
                }
                return _propertyInfo;
            }
        }
        private PropertyInfo _propertyInfo;

        [DataMember(IsRequired = true, EmitDefaultValue = true)]
        public PropertyAccessExpression Parent { get; private set; }

        public override string ToString()
        {
            return string.Format("{1}.{0}", PropertyName, Parent == null ? "$" : Parent.ToString().Trim().TrimStart('(').TrimEnd(')'));
        }
    }
}
