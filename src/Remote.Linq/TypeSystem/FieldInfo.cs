// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "Field")]
    public sealed class FieldInfo : MemberInfo
    {
        public FieldInfo(System.Reflection.FieldInfo fieldInfo)
            : base(fieldInfo)
        {
        }

        public FieldInfo(string fieldName, Type declaringType)
            : base(fieldName, declaringType)
        {
        }

        public override MemberTypes MemberType { get { return Remote.Linq.TypeSystem.MemberTypes.Field; } }

        private System.Reflection.FieldInfo SystemFieldInfo
        {
            get
            {
                if (ReferenceEquals(null, _systemFieldInfo))
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

                    var fieldInfo = declaringType.GetField(Name);
                    _systemFieldInfo = fieldInfo;
                }
                return _systemFieldInfo;
            }
        }
        [NonSerialized]
        private System.Reflection.FieldInfo _systemFieldInfo;

        public static implicit operator System.Reflection.FieldInfo(FieldInfo f)
        {
            return f.SystemFieldInfo;
        }
    }
}
