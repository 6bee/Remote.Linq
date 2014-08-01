// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "Method")]
    public class MethodInfo : MethodBaseInfo
    {
        public MethodInfo(System.Reflection.MethodInfo methodInfo)
            : base(methodInfo)
        {
            _method = methodInfo;
        }

        // TODO: replace binding flags by bool flags
        public MethodInfo(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes)
            : base(name, declaringType, bindingFlags, genericArguments, parameterTypes)
        {
        }

        public MethodInfo(string name, TypeInfo declaringType, BindingFlags bindingFlags, IEnumerable<TypeInfo> genericArguments, IEnumerable<TypeInfo> parameterTypes)
            : base(name, declaringType, bindingFlags, genericArguments, parameterTypes)
        {
        }

        public override MemberTypes MemberType { get { return TypeSystem.MemberTypes.Method; } }

        internal System.Reflection.MethodInfo Method
        {
            get
            {
                if (ReferenceEquals(null, _method))
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

                    var genericArguments = ReferenceEquals(null, GenericArgumentTypes) ? new Type[0] : GenericArgumentTypes
                        .Select(typeInfo =>
                        {
                            try
                            {
                                Type genericArgumentType = typeInfo;
                                return genericArgumentType;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("Generic argument type '{0}' could not be reconstructed", typeInfo), ex);
                            }
                        })
                        .ToArray();

                    var parameterTypes = ReferenceEquals(null, ParameterTypes) ? new Type[0] : ParameterTypes
                        .Select(typeInfo =>
                        {
                            try
                            {
                                Type parameterType = typeInfo;
                                return parameterType;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("Parameter type '{0}' could not be reconstructed", typeInfo), ex);
                            }
                        })
                        .ToArray();


                    var methodInfo = declaringType.GetMethod(Name, BindingFlags, null, parameterTypes, null);
                    //if (methodInfo == null) methodInfo = declaringType.GetMethod(MethodName);
                    if (ReferenceEquals(null, methodInfo))
                    {
                        methodInfo = declaringType.GetMethods(BindingFlags)
                            .Where(m => m.Name == Name)
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
                    _method = methodInfo;
                }
                return _method;
            }
        }
        [NonSerialized]
        private System.Reflection.MethodInfo _method;

        public override string ToString()
        {
            return string.Format("{0} {1}", new TypeInfo(Method.ReturnType), base.ToString());
        }

        public static implicit operator System.Reflection.MethodInfo(MethodInfo m)
        {
            return m.Method;
        }
    }
}
