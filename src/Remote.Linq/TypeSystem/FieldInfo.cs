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
            _field = fieldInfo;
        }

        public FieldInfo(string fieldName, Type declaringType)
            : base(fieldName, declaringType)
        {
        }

        public override MemberTypes MemberType { get { return Remote.Linq.TypeSystem.MemberTypes.Field; } }

        internal System.Reflection.FieldInfo Field
        {
            get
            {
                if (ReferenceEquals(null, _field))
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

                    var fieldInfo = declaringType.GetField(Name);
                    _field = fieldInfo;
                }
                return _field;
            }
        }
        [NonSerialized]
        private System.Reflection.FieldInfo _field;

        public static implicit operator System.Reflection.FieldInfo(FieldInfo f)
        {
            return f.Field;
        }
    }
}
