// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.TypeSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    internal static class TypeHelper
    {
        internal static Type GetElementType(Type type)
        {
            var enumerableType = FindIEnumerable(type);
            if (enumerableType == null)
            {
                return type;
            }

            return enumerableType.GetGenericArguments().First();
        }

        private static Type FindIEnumerable(Type type)
        {
            if (type == null || type == typeof(string))
            {
                return null;
            }

            if (type.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
            }

            if (type.IsGenericType())
            {
                foreach (var arg in type.GetGenericArguments())
                {
                    var enumerableType = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (enumerableType.IsAssignableFrom(type))
                    {
                        return enumerableType;
                    }
                }
            }

            var interfaces = type.GetInterfaces();
            if (interfaces != null && interfaces.Any())
            {
                foreach (var interfaceType in interfaces)
                {
                    var enumerableType = FindIEnumerable(interfaceType);
                    if (enumerableType != null)
                    {
                        return enumerableType;
                    }
                }
            }

            var baseType = type.GetBaseType();
            if (baseType != null && baseType != typeof(object))
            {
                return FindIEnumerable(baseType);
            }

            return null;
        }
    }
}
