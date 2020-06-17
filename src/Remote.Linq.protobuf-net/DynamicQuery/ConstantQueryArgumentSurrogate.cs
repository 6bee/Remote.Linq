// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.DynamicQuery
{
    using Aqua.ProtoBuf;
    using Aqua.ProtoBuf.Dynamic;
    using Aqua.TypeSystem;
    using global::ProtoBuf;
    using Remote.Linq.DynamicQuery;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ProtoContract(Name = nameof(ConstantQueryArgument))]
    public class ConstantQueryArgumentSurrogate
    {
        [ProtoMember(1)]
        public TypeInfo? Type { get; set; }

        [ProtoMember(2, OverwriteList = true)]
        [SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Required for serialization")]
        public Dictionary<string, Value?>? Properties { get; set; }

        [ProtoConverter]
        [return: NotNullIfNotNull("source")]
        public static ConstantQueryArgumentSurrogate? Convert(ConstantQueryArgument? source)
        {
            if (source is null)
            {
                return null;
            }

            var surrogate = DynamicObjectSurrogate.Convert(source);
            return new ConstantQueryArgumentSurrogate
            {
                Type = surrogate.Type,
                Properties = surrogate.Properties,
            };
        }

        [ProtoConverter]
        [return: NotNullIfNotNull("surrogate")]
        public static ConstantQueryArgument? Convert(ConstantQueryArgumentSurrogate? surrogate)
            => surrogate is null
            ? null
            : new ConstantQueryArgument(DynamicObjectSurrogate.Convert(surrogate));

        [SuppressMessage("Usage", "CA2225:Operator overloads have named alternates", Justification = "Surrogate type is not meant for direct usage")]
        public static implicit operator DynamicObjectSurrogate(ConstantQueryArgumentSurrogate surrogate)
            => new DynamicObjectSurrogate
            {
                Type = surrogate.CheckNotNull(nameof(surrogate)).Type,
                Properties = surrogate.Properties,
            };
    }
}
