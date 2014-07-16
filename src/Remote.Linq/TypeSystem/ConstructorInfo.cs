// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;
using System.Runtime.Serialization;
using BindingFlags = System.Reflection.BindingFlags;

namespace Remote.Linq.TypeSystem
{
    [Serializable]
    [DataContract(Name = "Constructor")]
    public class ConstructorInfo : MethodBaseInfo
    {
        public ConstructorInfo(System.Reflection.ConstructorInfo constructorInfo)
            : base(constructorInfo)
        {
            _systemConstructorInfo = constructorInfo;
        }

        // TODO: replace binding flags by bool flags
        public ConstructorInfo(string name, Type declaringType, BindingFlags bindingFlags, Type[] genericArguments, Type[] parameterTypes)
            : base(name, declaringType, bindingFlags, genericArguments, parameterTypes)
        {
        }

        public override MemberTypes MemberType { get { return TypeSystem.MemberTypes.Constructor; } }

        private System.Reflection.ConstructorInfo SystemConstructorInfo
        {
            get
            {
                if (ReferenceEquals(null, _systemConstructorInfo))
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

                    var genericArguments = ReferenceEquals(null, GenericArgumentTypes) ? new Type[0] : GenericArgumentTypes
                        .Select(typeInfo =>
                        {
                            try
                            {
                                Type genericArgumentType = typeInfo;
                                return genericArgumentType;
                            }
                            catch
                            {
                                throw new Exception(string.Format("Generic argument type '{0}' could not be reconstructed", typeInfo));
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
                            catch
                            {
                                throw new Exception(string.Format("Parameter type '{0}' could not be reconstructed", typeInfo));
                            }
                        })
                        .ToArray();

                    var constructorInfo = declaringType.GetConstructor(BindingFlags, null, parameterTypes, null);
                    if (ReferenceEquals(null, constructorInfo))
                    {
                        constructorInfo = declaringType.GetConstructors(BindingFlags)
                            .Where(m => m.Name == Name)
                            .Where(m => !m.IsGenericMethod || m.GetGenericArguments().Length == genericArguments.Length)
                            .Where(m => m.GetParameters().Length == parameterTypes.Length)
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
                    _systemConstructorInfo = constructorInfo;
                }
                return _systemConstructorInfo;
            }
        }
        [NonSerialized]
        private System.Reflection.ConstructorInfo _systemConstructorInfo;

        public override string ToString()
        {
            return string.Format(".ctor {1}", base.ToString());
        }

        public static implicit operator System.Reflection.ConstructorInfo(ConstructorInfo c)
        {
            return c.SystemConstructorInfo;
        }
    }
}
