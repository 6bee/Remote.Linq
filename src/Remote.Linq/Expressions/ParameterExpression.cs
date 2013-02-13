// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
#if !SILVERLIGHT
    [Serializable]
#endif
    [DataContract]
    public sealed class ParameterExpression : Expression
    {
        internal ParameterExpression(string parameterName, Type type)
        {
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
                if (ReferenceEquals(_parameterType, null))
                {
                    _parameterType = Type.GetType(ParameterTypeName);
#if !SILVERLIGHT
                    if (ReferenceEquals(_parameterType, null))
                    {
                        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                        {
                            _parameterType = assembly.GetType(ParameterTypeName);
                            if (!ReferenceEquals(_parameterType, null)) break;
                        }
                    }
#endif
                }
                return _parameterType;
            }
        }
        private Type _parameterType;

        public override string ToString()
        {
            return string.Format("{0}", ParameterName);
        }
    }
}
