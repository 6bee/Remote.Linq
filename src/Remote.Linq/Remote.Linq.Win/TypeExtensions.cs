// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using BindingFlags = System.Reflection.BindingFlags;
    using MemberTypes = Remote.Linq.TypeSystem.MemberTypes;

    internal static class TypeExtensions
    {
        public static Type GetUnderlyingSystemType(this Type type)
        {
            // UnderlyingSystemType is not supported by WinRT
            return type;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.GetTypeInfo().IsGenericType;
        }

        public static bool IsEnum(this Type type)
        {
            return type.GetTypeInfo().IsEnum;
        }

        public static bool IsValueType(this Type type)
        {
            return type.GetTypeInfo().IsValueType;
        }

        public static bool IsSerializable(this Type type)
        {
            return type.GetTypeInfo().IsSerializable;
        }

        public static IEnumerable<Type> GetInterfaces(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces;
        }

        public static Type GetBaseType(this Type type)
        {
            return type.GetTypeInfo().BaseType;
        }

        public static IEnumerable<Type> GetGenericArguments(this Type type)
        {
            var genericTypeArguments = type.GetTypeInfo().GenericTypeArguments;
            return genericTypeArguments;
        }

        public static IEnumerable<System.Reflection.ConstructorInfo> GetConstructors(this Type type)
        {
            return type.GetTypeInfo().DeclaredConstructors;
        }

        public static IEnumerable<System.Reflection.ConstructorInfo> GetConstructors(this Type type, BindingFlags bindingAttr)
        {
            return type.GetTypeInfo().DeclaredConstructors.Filter(bindingAttr);
        }

        public static System.Reflection.ConstructorInfo GetConstructor(this Type type, BindingFlags bindingAttr, /*Binder*/object binder, Type[] types, /*ParameterModifier[]*/object modifiers)
        {
            if (!ReferenceEquals(null, binder)) throw new NotSupportedException("Binder not supported by WinRT");
            if (!ReferenceEquals(null, modifiers)) throw new NotSupportedException("ParameterModifier not supported by WinRT");

            var constructors = type.GetTypeInfo().DeclaredConstructors
                .Filter(bindingAttr)
                .Where(c => ParametersMatch(c, types))
                .ToList();

            switch (constructors.Count)
            {
                case 0:
                    return null;
                case 1:
                    return constructors[0];
                default:
                    throw new AmbiguousMatchException("More than one construtor found matching the specified binding constraints. (Note: binding flags are not supported by WinRT)");
            }
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this Type type, Type attributeType, bool inherit)
        {
            return type.GetTypeInfo().GetCustomAttributes(attributeType, inherit);
        }

        public static bool IsAnonymousType(this Type type)
        {
            return type.Name.StartsWith("<>")
                && type.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Any();
        }

        public static IEnumerable<System.Reflection.MemberInfo> GetMember(this Type type, string name, MemberTypes memberType, BindingFlags bindingAttr)
        {
            // Note: binding flags are simply ignored
            var members = new List<System.Reflection.MemberInfo>();

            if ((memberType & MemberTypes.Constructor) == MemberTypes.Constructor)
            {
                members.AddRange(type.GetTypeInfo().DeclaredConstructors.Where(x => string.Compare(x.Name, name) == 0));
            }

            if ((memberType & MemberTypes.Field) == MemberTypes.Field)
            {
                members.Add(type.GetTypeInfo().GetDeclaredField(name));
            }

            if ((memberType & MemberTypes.Method) == MemberTypes.Method)
            {
                members.AddRange(type.GetTypeInfo().GetDeclaredMethods(name));
            }

            if ((memberType & MemberTypes.Property) == MemberTypes.Property)
            {
                members.Add(type.GetTypeInfo().GetDeclaredProperty(name));
            }

            return members.ToArray();
        }

        public static System.Reflection.ConstructorInfo GetConstructor(this Type type, Type[] types)
        {
            var constructors = type.GetTypeInfo().DeclaredConstructors
                .Where(m => ParametersMatch(m, types))
                .ToList();

            switch (constructors.Count)
            {
                case 0:
                    return null;
                case 1:
                    return constructors[0];
                default:
                    throw new AmbiguousMatchException("More than one constructor is found with the specified name and parameters.");
            }
        }

        public static System.Reflection.FieldInfo GetField(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredField(name);
        }

        public static IEnumerable<System.Reflection.MethodInfo> GetMethods(this Type type, BindingFlags bindingAttr)
        {
            var methods = type.GetTypeInfo().DeclaredMethods.Filter(bindingAttr);
            return methods.ToArray();
        }

        public static System.Reflection.MethodInfo GetMethod(this Type type, string name)
        {
            var methods = type.GetTypeInfo().DeclaredMethods
                .Where(m => string.Compare(m.Name, name) == 0)
                .ToList();

            switch (methods.Count)
            {
                case 0:
                    return null;
                case 1:
                    return methods[0];
                default:
                    throw new AmbiguousMatchException("More than one method is found with the specified name.");
            }
        }

        public static System.Reflection.MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            var methods = type.GetTypeInfo().DeclaredMethods
                .Where(m => string.Compare(m.Name, name) == 0)
                .Where(m => ParametersMatch(m, types))
                .ToList();

            switch (methods.Count)
            {
                case 0:
                    return null;
                case 1:
                    return methods[0];
                default:
                    throw new AmbiguousMatchException("More than one method is found with the specified name and parameters.");
            }
        }

        public static System.Reflection.MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr)
        {
            var methods = type.GetTypeInfo().GetDeclaredMethods(name)
                .Filter(bindingAttr)
                .ToList();

            switch (methods.Count)
            {
                case 0:
                    return null;
                case 1:
                    return methods[0];
                default:
                    throw new AmbiguousMatchException("More than one method is found with the specified name and matching the specified binding constraints.");
            }
        }

        public static System.Reflection.MethodInfo GetMethod(this Type type, BindingFlags bindingAttr)
        {
            var methods = type.GetTypeInfo().DeclaredMethods
                .Filter(bindingAttr)
                .ToList();

            switch (methods.Count)
            {
                case 0:
                    return null;
                case 1:
                    return methods[0];
                default:
                    throw new AmbiguousMatchException("More than one method is found with the specified name and matching the specified binding constraints.");
            }
        }

        public static System.Reflection.MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr, /*Binder*/object binder, Type[] types, /*ParameterModifier[]*/object modifiers)
        {
            if (!ReferenceEquals(null, binder)) throw new NotSupportedException("Binder not supported by WinRT");
            if (!ReferenceEquals(null, modifiers)) throw new NotSupportedException("ParameterModifier not supported by WinRT");

            var methods = type.GetTypeInfo().GetDeclaredMethods(name)
                .Filter(bindingAttr)
                .Where(m => ParametersMatch(m, types))
                .ToList();

            switch (methods.Count)
            {
                case 0:
                    return null;
                case 1:
                    return methods[0];
                default:
                    throw new AmbiguousMatchException("More than one method is found with the specified name and matching the specified binding constraints.");
            }
        }

        public static System.Reflection.PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static System.Reflection.PropertyInfo GetProperty(this Type type, string name, Type returnType)
        {
            var properties = type.GetProperties()
                .Where(p => string.Compare(p.Name, name) == 0 && p.PropertyType == returnType)
                .ToList();

            switch (properties.Count)
            {
                case 0:
                    return null;
                case 1:
                    return properties[0];
                default:
                    throw new AmbiguousMatchException("More than one property is found with the specified name.");
            }
        }

        public static System.Reflection.PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingAttr)
        {
            var properties = type.GetProperties()
                .Where(p => string.Compare(p.Name, name) == 0)
                .Filter(bindingAttr)
                .ToList();

            switch (properties.Count)
            {
                case 0:
                    return null;
                case 1:
                    return properties[0];
                default:
                    throw new AmbiguousMatchException("More than one property is found with the specified name and matching the specified binding constraints.");
            }
        }

        public static IEnumerable<System.Reflection.PropertyInfo> GetProperties(this Type type)
        {
            return type.GetTypeInfo().DeclaredProperties;
        }

        public static IEnumerable<System.Reflection.PropertyInfo> GetProperties(this Type type, BindingFlags bindingAttr)
        {
            return type.GetProperties()
                .Filter(bindingAttr)
                .ToList();
        }

        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }

        private static bool ParametersMatch(System.Reflection.MethodBase m, Type[] types)
        {
            var parameters = m.GetParameters();
            if (parameters.Length == types.Length)
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i].ParameterType != types[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }

        private static IEnumerable<T> Filter<T>(this IEnumerable<T> memberInfos, BindingFlags bindingAttr) where T : System.Reflection.MemberInfo
        {
            // Note: binding flags are simply ignored
            return memberInfos;
        }
    }
}
