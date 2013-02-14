// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Reflection;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class PropertyAccessExpression : Expression
    {
        internal PropertyAccessExpression(Expression instance, PropertyInfo propertyInfo)
            : this(instance, propertyInfo.Name, propertyInfo.PropertyType, propertyInfo.DeclaringType)
        {
        }

        internal PropertyAccessExpression(Expression instance, string propertyName, Type propertyType, Type declaringType)
        {
            Instance = instance;
            PropertyName = propertyName;
            PropertyType = propertyType.FullName;//.AssemblyQualifiedName;
            DeclaringType = declaringType.FullName;//.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.PropertyAccess; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string PropertyName { get; private set; }
#else
        private string PropertyName { get; set; }
#endif

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string PropertyType { get; private set; }
#else
        private string PropertyType { get; set; }
#endif

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string DeclaringType { get; private set; }
#else
        private string DeclaringType { get; set; }
#endif

        public PropertyInfo PropertyInfo
        {
            get
            {
                if (ReferenceEquals(_propertyInfo, null))
                {
                    var declaringType = Type.GetType(DeclaringType);
                    if (ReferenceEquals(declaringType, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            declaringType = assembly.GetType(DeclaringType);
                            if (!ReferenceEquals(declaringType, null)) break;
                        }
                    }
                    var propertyType = Type.GetType(PropertyType);
                    if (ReferenceEquals(propertyType, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            propertyType = assembly.GetType(PropertyType);
                            if (!ReferenceEquals(propertyType, null)) break;
                        }
                    }
                    var propertyInfo = declaringType.GetProperty(PropertyName, propertyType);
                    if (propertyInfo == null) propertyInfo = declaringType.GetProperty(PropertyName);
                    _propertyInfo = propertyInfo;
                }
                return _propertyInfo;
            }
        }
        private PropertyInfo _propertyInfo;

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Expression Instance { get; private set; }

        public override string ToString()
        {
            return string.Format("{0}.{1}", Instance == null ? DeclaringType : Instance.ToString(), PropertyName);
        }
    }
}
