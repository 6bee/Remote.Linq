// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text.RegularExpressions;

    partial class DynamicObjectMapper
    {
        /// <summary>
        /// .NET platform specific regex options
        /// </summary>
        private const RegexOptions LocalRegexOptions = RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline;

        private static readonly Regex _backingFieldRegex = new Regex(@"^(.+\+)?\<(?<name>.+)\>k__BackingField$", LocalRegexOptions);

        /// <summary>
        /// Gets an uninitialized instance of the specified type by using <see cref="FormatterServices" />
        /// </summary>
        private static object GetUninitializedObject(Type type)
        {
            return FormatterServices.GetUninitializedObject(type);
        }

        /// <summary>
        /// Populate object members type by using <see cref="FormatterServices" />
        /// </summary>
        private void PopulateObjectMembers(Type type, DynamicObject from, object to)
        {
            var customPropertySet = GetPropertiesForMapping(type);
            var customPropertyNames = ReferenceEquals(null, customPropertySet) ? null : customPropertySet.ToDictionary(x => x.Name);

            var members = FormatterServices.GetSerializableMembers(type);
            var membersByCleanName = members.ToDictionary(x => CleanBackingFieldNameIfRequired(x.Name));
            var memberValueMap = new Dictionary<System.Reflection.MemberInfo, object>();

            foreach (var dynamicProperty in from)
            {
                if (!ReferenceEquals(null, customPropertyNames) && !customPropertyNames.ContainsKey(dynamicProperty.Name))
                {
                    continue;
                }

                System.Reflection.MemberInfo member;
                if (membersByCleanName.TryGetValue(dynamicProperty.Name, out member))
                {
                    Type memberType;
                    switch (member.MemberType)
                    {
                        case System.Reflection.MemberTypes.Field:
                            memberType = ((System.Reflection.FieldInfo)member).FieldType;
                            break;

                        case System.Reflection.MemberTypes.Property:
                            memberType = ((System.Reflection.PropertyInfo)member).PropertyType;
                            break;

                        default:
                            throw new Exception(string.Format("Unsupported member type {0}.", member.MemberType));
                    }

                    var value = MapFromDynamicObjectGraph(dynamicProperty.Value, memberType);

                    if (_suppressMemberAssignabilityValidation || IsAssignable(memberType, value))
                    {
                        memberValueMap[member] = value;
                    }
                }
            }

            FormatterServices.PopulateObjectMembers(to, memberValueMap.Keys.ToArray(), memberValueMap.Values.ToArray());
        }

        /// <summary>
        /// Retrieves object members type by using <see cref="FormatterServices" /> and populates dynamic object
        /// </summary>
        private void MapObjectMembers(object from, DynamicObject to, bool setTypeInformation)
        {
            var type = _typeResolver.ResolveType(to.Type);

            var customPropertySet = GetPropertiesForMapping(type);
            var customPropertyNames = ReferenceEquals(null, customPropertySet) ? null : customPropertySet.ToDictionary(x => x.Name);
            var members = FormatterServices.GetSerializableMembers(type);
            var values = FormatterServices.GetObjectData(from, members);
            for (int i = 0; i < members.Length; i++)
            {
                var memberName = CleanBackingFieldNameIfRequired(members[i].Name);
                if (!ReferenceEquals(null, customPropertyNames) && !customPropertyNames.ContainsKey(memberName))
                {
                    continue;
                }

                var value = MapToDynamicObjectIfRequired(values[i], setTypeInformation);
                to[memberName] = value;
            }
        }

        private static string CleanBackingFieldNameIfRequired(string memberName)
        {
            var match = _backingFieldRegex.Match(memberName);
            if (match.Success && match.Groups.Count == 2)
            {
                memberName = match.Groups[1].Value;
            }
            return memberName;
        }
    }
}