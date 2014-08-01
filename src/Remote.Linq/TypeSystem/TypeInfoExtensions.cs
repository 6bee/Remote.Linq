// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Remote.Linq.TypeSystem
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class TypeInfoExtensions
    {
        public static Type GetGenericTypeDefinition(this TypeInfo typeInfo)
        {
            return typeInfo.Type.GetGenericTypeDefinition();
        }

        public static IEnumerable<Type> GetGenericArguments(this TypeInfo typeInfo)
        {
            return typeInfo.Type.GetGenericArguments();
        }

        public static System.Reflection.ConstructorInfo GetConstructor(this TypeInfo typeInfo, Type[] types)
        {
            return typeInfo.Type.GetConstructor(types);
        }
    }
}