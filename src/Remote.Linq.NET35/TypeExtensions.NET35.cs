// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using System.Reflection;

    partial class TypeExtensions
    {
        public static void SetValue(this PropertyInfo property, object obj, object value)
        {
            property.SetValue(obj, value, null);
        }

        public static object GetValue(this PropertyInfo property, object obj)
        {
            return property.GetValue(obj, null);
        }
    }
}
