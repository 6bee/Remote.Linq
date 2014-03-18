// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class ParameterExpression : Expression
    {
        internal ParameterExpression(string parameterName, Type type)
        {
            _parameterType = type;
            ParameterName = parameterName;
            ParameterTypeName = type.FullName;//.AssemblyQualifiedName;
        }

        public override ExpressionType NodeType { get { return ExpressionType.Parameter; } }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
        public string ParameterName { get; private set; }

        [DataMember(Name = "ParameterType", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        public string ParameterTypeName { get; private set; }
#else
        private string ParameterTypeName { get; set; }
#endif

        public Type ParameterType
        {
            get
            {
                if (ReferenceEquals(null, _parameterType))
                {
                    try
                    {
                        _parameterType = TypeResolver.Instance.ResolveType(ParameterTypeName);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Parameter type '{0}' could not be reconstructed", ParameterTypeName));
                    }
                }
                return _parameterType;
            }
        }
        [NonSerialized]
        private Type _parameterType;

        public override string ToString()
        {
            return string.Format("{0}", ParameterName);
        }
    }
}
