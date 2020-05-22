// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf
{
    using global::ProtoBuf.Meta;
    using System;
    using System.Linq;

    internal static class ProtoBufMetaExtensions
    {
        public static MetaType GetType<T>(this RuntimeTypeModel typeModel)
            => typeModel[typeof(T)];

        public static ValueMember GetMember(this MetaType type, string propertyName)
            => type.GetFields().Single(x => string.Equals(x.Name, propertyName, StringComparison.Ordinal));

        public static void SetSurrogate<T>(this MetaType type)
            => type.SetSurrogate(typeof(T));

        public static ValueMember MakeDynamicType(this ValueMember member)
        {
            member.DynamicType = true;
            return member;
        }
    }
}
