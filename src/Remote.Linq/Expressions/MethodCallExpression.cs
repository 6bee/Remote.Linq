// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace Remote.Linq.Expressions
{
    [Serializable]
    [DataContract]
    public sealed class MethodCallExpression : Expression
    {
        [DataMember(Name = "Arguments", IsRequired = false, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal readonly Expression[] _arguments;
#else
        private readonly Expression[] _arguments;
#endif

        internal MethodCallExpression(Expression insatnce, MethodInfo methodInfo, IEnumerable<Expression> arguments)
            : this(insatnce, methodInfo.Name, methodInfo.DeclaringType, BindingFlags.Default, methodInfo.GetGenericArguments(), methodInfo.GetParameters().Select(p => p.ParameterType).ToArray(), arguments)
        {
            _methodInfo = methodInfo;
            var bindingFlags = methodInfo.IsStatic ? BindingFlags.Static : BindingFlags.Instance;
            bindingFlags |= methodInfo.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
            BindingFlags = bindingFlags;
        }

        internal MethodCallExpression(Expression insatnce, string methodName, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes, IEnumerable<Expression> arguments)
        {
            Instance = insatnce;
            MethodName = methodName;
            DeclaringTypeName = declaringType.FullName;//.AssemblyQualifiedName;
            GenericArgumentNames = genericArguments.Length == 0 ? null : genericArguments.Select(p => p.FullName).ToArray();
            ParameterTypeNames = parameterTypes.Length == 0 ? null : parameterTypes.Select(p => p.FullName).ToArray();
            _arguments = arguments.ToArray();
            BindingFlags = bindingFlags;
        }

        public override ExpressionType NodeType { get { return ExpressionType.MethodCall; } }

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
        public Expression Instance { get; private set; }

        [DataMember(IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal string MethodName { get; private set; }
#else
        private string MethodName { get; set; }
#endif

        [DataMember(Name = "DeclaringType", IsRequired = true, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal string DeclaringTypeName { get; private set; }
#else
        private string DeclaringTypeName { get; set; }
#endif

        [DataMember(Name = "ParameterTypes", IsRequired = false, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal string[] ParameterTypeNames { get; private set; }
#else
        private string[] ParameterTypeNames { get; set; }
#endif

        [DataMember(Name = "GenericArguments", IsRequired = false, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal string[] GenericArgumentNames { get; private set; }
#else
        private string[] GenericArgumentNames { get; set; }
#endif

        [DataMember(IsRequired = false, EmitDefaultValue = false)]
#if SILVERLIGHT
        internal BindingFlags BindingFlags { get; private set; }
#else
        private BindingFlags BindingFlags { get; set; }
#endif

        public MethodInfo MethodInfo
        {
            get
            {
                if (ReferenceEquals(_methodInfo, null))
                {
                    Type declaringType;
                    try
                    {
                        declaringType = TypeResolver.Instance.ResolveType(DeclaringTypeName);
                    }
                    catch
                    {
                        throw new Exception(string.Format("Declaring type '{0}' could not be reconstructed", DeclaringTypeName));
                    }

                    var genericArguments = GenericArgumentNames == null ? new Type[0] : GenericArgumentNames
                        .Select(a =>
                        {
                            try
                            {
                                var genericArgumentType = TypeResolver.Instance.ResolveType(a);
                                return genericArgumentType;
                            }
                            catch
                            {
                                throw new Exception(string.Format("Generic argument type '{0}' could not be reconstructed", a));
                            }
                        })
                        .ToArray();

                    var parameterTypes = ParameterTypeNames == null ? new Type[0] : ParameterTypeNames
                        .Select(p =>
                        {
                            try
                            {
                                var parameterType = TypeResolver.Instance.ResolveType(p);
                                return parameterType;
                            }
                            catch
                            {
                                throw new Exception(string.Format("Parameter type '{0}' could not be reconstructed", p));
                            }
                        })
                        .ToArray();


                    var methodInfo = declaringType.GetMethod(MethodName, BindingFlags, null, parameterTypes, null);
                    //if (methodInfo == null) methodInfo = declaringType.GetMethod(MethodName);
                    if (methodInfo == null)
                    {
                        methodInfo = declaringType.GetMethods(BindingFlags)
                            .Where(m => m.Name == MethodName)
                            .Where(m => !m.IsGenericMethod || m.GetGenericArguments().Length == genericArguments.Length)
                            .Where(m => m.GetParameters().Length == parameterTypes.Length)
                            .Select(m => m.IsGenericMethod ? m.MakeGenericMethod(genericArguments) : m)
                            .Where(m =>
                            {
                                var paramTypes = m.GetParameters();
                                for (int i = 0; i < parameterTypes.Length; i++)
                                {
                                    if (paramTypes[i].ParameterType != parameterTypes[i]) return false;
                                }
                                return true;
                            })
                            .Single();
                    }
                    _methodInfo = methodInfo;
                }
                return _methodInfo;
            }
        }
        [NonSerialized]
        private MethodInfo _methodInfo;

        public Expression[] Arguments { get { return _arguments.ToArray(); } }

        public override string ToString()
        {
            return string.Format("{0}.{1}({2})",
                Instance == null ? DeclaringTypeName : Instance.ToString(), 
                MethodName, 
                // ParameterTypes == null ? null : string.Join(", ", ParameterTypes)
                _arguments == null ? null : string.Join(", ", _arguments.Select(a => a.ToString()).ToArray()));
        }
    }
}
