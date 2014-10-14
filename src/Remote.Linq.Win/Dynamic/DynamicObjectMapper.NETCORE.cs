// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace Remote.Linq.Dynamic
{
    partial class DynamicObjectMapper
    {
        /// <summary>
        /// Platform specific regex options (WinRT, WP, SL)
        /// </summary>
        private const RegexOptions LocalRegexOptions = RegexOptions.ExplicitCapture | RegexOptions.Singleline;

        /// <summary>
        /// Not supported for this platform (WinRT, WP, SL)
        /// </summary>
        private static object GetUninitializedObject(Type type)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported for this platform (WinRT, WP, SL)
        /// </summary>
        private static void PopulateObjectMembers(Type type, DynamicObject from, object to)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Not supported for this platform (WinRT, WP, SL)
        /// </summary>
        private static void MapObjectMembers(object from, DynamicObject to, bool setTypeInformation)
        {
            throw new NotSupportedException();
        }
    }
}
