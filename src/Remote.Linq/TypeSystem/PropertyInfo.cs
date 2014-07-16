// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "Property")]
    public sealed class PropertyInfo : MemberInfo
    {
        public PropertyInfo(System.Reflection.PropertyInfo propertyInfo)
            : base(propertyInfo)
        {
            _systemPropertyInfo = propertyInfo;
            PropertyType = new TypeInfo(propertyInfo.PropertyType);
        }

        public PropertyInfo(string propertyName, Type propertyType, Type declaringType)
            : base(propertyName, declaringType)
        {
            PropertyType = new TypeInfo(propertyType);
        }

        public override MemberTypes MemberType { get { return Remote.Linq.TypeSystem.MemberTypes.Property; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public TypeInfo PropertyType { get; private set; }

        private System.Reflection.PropertyInfo SystemPropertyInfo
        {
            get
            {
                if (ReferenceEquals(null, _systemPropertyInfo))
                {
                    Type declaringType;
                    try
                    {
                        declaringType = DeclaringType;
                    }
                    catch
                    {
                        throw new Exception(string.Format("Declaring type '{0}' could not be reconstructed", DeclaringType));
                    }

                    Type propertyType;
                    try
                    {
                        propertyType = PropertyType;
                    }
                    catch
                    {
                        throw new Exception(string.Format("Property type '{0}' could not be reconstructed", PropertyType));
                    }

                    var propertyInfo = declaringType.GetProperty(Name, propertyType);
                    if (propertyInfo == null) propertyInfo = declaringType.GetProperty(Name);
                    _systemPropertyInfo = propertyInfo;
                }
                return _systemPropertyInfo;
            }
        }
        [NonSerialized]
        private System.Reflection.PropertyInfo _systemPropertyInfo;

        public static implicit operator System.Reflection.PropertyInfo(PropertyInfo p)
        {
            return p.SystemPropertyInfo;
        }
    }
}
