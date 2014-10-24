// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Remote.Linq
{
    internal static partial class TypeExtensions
    {
        public static Type GetUnderlyingSystemType(this Type type)
        {
            return type.UnderlyingSystemType;
        }

        public static bool IsGenericType(this Type type)
        {
            return type.IsGenericType;
        }

        public static bool IsAnonymousType(this Type type)
        {
            return type.Name.StartsWith("<>")
                && type.GetCustomAttributes(typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute), false).Any();
        }

        public static bool IsEnum(this Type type)
        {
            return type.IsEnum;
        }

        public static bool IsValueType(this Type type)
        {
            return type.IsValueType;
        }

        public static bool IsSerializable(this Type type)
        {
#if SILVERLIGHT
            return false;
#else
            return type.IsSerializable;
#endif
        }

        public static Type GetBaseType(this Type type)
        {
            return type.BaseType;
        }

        public static IEnumerable<System.Reflection.MemberInfo> GetMember(this Type type, string name, Remote.Linq.TypeSystem.MemberTypes memberType, System.Reflection.BindingFlags bindingFlags)
        {
            var t = (System.Reflection.MemberTypes)memberType;
            return type.GetMember(name, t, bindingFlags);
        }
    }
}
