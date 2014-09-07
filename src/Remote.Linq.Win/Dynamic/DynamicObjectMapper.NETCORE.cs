// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Text.RegularExpressions;

namespace Remote.Linq.Dynamic
{
    static partial class DynamicObjectMapper
    {
        /// <summary>
        /// Platform specific regex options (WinRT, WP, SL)
        /// </summary>
        private const RegexOptions BackingFieldRegexOptions = RegexOptions.ExplicitCapture | RegexOptions.Singleline;

        /// <summary>
        /// Not supported for this platform (WinRT, WP, SL)
        /// </summary>
        private static object GetUninitializedObject(Type type)
        {
            throw new NotSupportedException();
            //return FormatterServices.GetUninitializedObject(type);
        }

        /// <summary>
        /// Not supported for this platform (WinRT, WP, SL)
        /// </summary>
        private static void PopulateObjectMembers(Type type, DynamicObject from, object to, ObjectFormatterContext<DynamicObject, object> referenceMap)
        {
            throw new NotSupportedException();
            //var members = FormatterServices.GetSerializableMembers(to.GetType());
            //var values = from.Values
            //    .Select(x => MapDynamicObjectIfRequired(x, referenceMap))
            //    .ToArray();
            //FormatterServices.PopulateObjectMembers(to, members, values);
        }

        /// <summary>
        /// Not supported for this platform (WinRT, WP, SL)
        /// </summary>
        private static void MapObjectMembers(object from, DynamicObject to, ObjectFormatterContext<object, DynamicObject> map)
        {
            throw new NotSupportedException();
            //var type = to.Type.Type;

            //var members = FormatterServices.GetSerializableMembers(type);
            //var values = FormatterServices.GetObjectData(from, members);
            //for (int i = 0; i < members.Length; i++)
            //{
            //    var memberName = members[i].Name;
            //    var match = BackingFieldRegex.Match(memberName);
            //    if (match.Success && match.Groups.Count == 2)
            //    {
            //        memberName = match.Groups[1].Value;
            //    }
            //    var value = MapValueIfRequired(values[i], map);
            //    to[memberName] = value;
            //}
        }
    }
}
