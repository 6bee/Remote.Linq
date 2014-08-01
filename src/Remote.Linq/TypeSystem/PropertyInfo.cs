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
            _property = propertyInfo;
        }

        public PropertyInfo(string propertyName, Type declaringType)
            : base(propertyName, declaringType)
        {
        }

        public override MemberTypes MemberType { get { return Remote.Linq.TypeSystem.MemberTypes.Property; } }

        internal System.Reflection.PropertyInfo Property
        {
            get
            {
                if (ReferenceEquals(null, _property))
                {
                    Type declaringType;
                    try
                    {
                        declaringType = DeclaringType;
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(string.Format("Declaring type '{0}' could not be reconstructed", DeclaringType), ex);
                    }

                    var propertyInfo = declaringType.GetProperty(Name);
                    _property = propertyInfo;
                }
                return _property;
            }
        }
        [NonSerialized]
        private System.Reflection.PropertyInfo _property;

        public static implicit operator System.Reflection.PropertyInfo(PropertyInfo p)
        {
            return p.Property;
        }
    }
}
