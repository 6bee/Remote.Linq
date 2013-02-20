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
            PropertyTypeName = propertyType.FullName;//.AssemblyQualifiedName;
            DeclaringTypeName = declaringType.FullName;//.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.PropertyAccess; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string PropertyName { get; private set; }
#else
        private string PropertyName { get; set; }
#endif

        [DataMember(Name = "PropertyType", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string PropertyTypeName { get; private set; }
#else
        private string PropertyTypeName { get; set; }
#endif

        [DataMember(Name = "DeclaringType", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string DeclaringTypeName { get; private set; }
#else
        private string DeclaringTypeName { get; set; }
#endif

        public PropertyInfo PropertyInfo
        {
            get
            {
                if (ReferenceEquals(_propertyInfo, null))
                {
                    var declaringType = Type.GetType(DeclaringTypeName);
                    if (ReferenceEquals(declaringType, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            declaringType = assembly.GetType(DeclaringTypeName);
                            if (!ReferenceEquals(declaringType, null)) break;
                        }
                        if (ReferenceEquals(declaringType, null))
                        {
                            throw new Exception(string.Format("Declaring type '{0}' could not be reconstructed", DeclaringTypeName));
                        }
                    }
                    var propertyType = Type.GetType(PropertyTypeName);
                    if (ReferenceEquals(propertyType, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            propertyType = assembly.GetType(PropertyTypeName);
                            if (!ReferenceEquals(propertyType, null)) break;
                        }
                        if (ReferenceEquals(declaringType, null))
                        {
                            throw new Exception(string.Format("Property type '{0}' could not be reconstructed", PropertyTypeName));
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
            return string.Format("{0}.{1}", Instance == null ? DeclaringTypeName : Instance.ToString(), PropertyName);
        }
    }
}
