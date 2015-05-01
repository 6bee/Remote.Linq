// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq
{
    using ProtoBuf.Meta;
    using System.Linq;

    internal static class ProtoBufMetaExtensions
    {
        public static MetaType GetType<T>(this RuntimeTypeModel typeModel)
        {
            return typeModel[typeof(T)];
        }

        public static ValueMember GetMember(this MetaType type, string propertyName)
        {
            return type.GetFields().Single(x => string.Equals(x.Name, propertyName));
        }

        public static ValueMember MakeDynamicType(this ValueMember member)
        {
            member.DynamicType = true;

            return member;
        }
    }
}
