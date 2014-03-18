// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Remote.Linq
{
    internal static class TypeExtensions
    {
        public static Type GetUnderlyingSystemType(this Type type)
        {
            // UnderlyingSystemType is not supported by WinRT
            return type;
        }

        public static Type[] GetGenericArguments(this Type type)
        {
            var genericTypeArguments = type.GetTypeInfo().GenericTypeArguments;
            return genericTypeArguments;
        }

        public static MethodInfo[] GetMethods(this Type type, BindingFlags bindingAttr)
        {
            var methods = type.GetTypeInfo().DeclaredMethods.Filter(bindingAttr);
            return methods.ToArray();
        }

        public static MethodInfo GetMethod(this Type type, string name, Type[] types)
        {
            // TODO: filter by types array
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
                    throw new AmbiguousMatchException("More than one method is found with the specified name and specified parameters.");
            }
        }

        public static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr)
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

        public static MethodInfo GetMethod(this Type type, string name, BindingFlags bindingAttr, /*Binder*/object binder, Type[] types, /*ParameterModifier[]*/object modifiers)
        {
            if (!ReferenceEquals(null, binder)) throw new NotSupportedException("Binder not supported by WinRT");
            if (!ReferenceEquals(null, modifiers)) throw new NotSupportedException("ParameterModifier not supported by WinRT");

            // TODO: filter by types array
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

        private static IEnumerable<T> Filter<T>(this IEnumerable<T> memberInfos, BindingFlags bindingAttr) where T : MemberInfo
        {
            // TODO: filter by binding flags
            return memberInfos;
        }

        public static PropertyInfo GetProperty(this Type type, string name)
        {
            return type.GetTypeInfo().GetDeclaredProperty(name);
        }

        public static PropertyInfo GetProperty(this Type type, string name, Type returnType)
        {
            var properties = type.GetTypeInfo().DeclaredProperties
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

        public static PropertyInfo GetProperty(this Type type, string name, BindingFlags bindingAttr)
        {
            var properties = type.GetTypeInfo().DeclaredProperties
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

        public static bool IsAssignableFrom(this Type type, Type c)
        {
            return type.GetTypeInfo().IsAssignableFrom(c.GetTypeInfo());
        }
    }
}