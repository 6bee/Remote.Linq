// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.DynamicQuery
{
    using Aqua.ProtoBuf;
    using Aqua.TypeSystem;
    using global::ProtoBuf;
    using Remote.Linq.DynamicQuery;

    [ProtoContract(Name = nameof(VariableQueryArgument))]
    public class VariableQueryArgumentSurrogate
    {
        [ProtoMember(1, IsRequired = true)]
        public TypeInfo Type { get; set; } = null!;

        [ProtoMember(2)]
        public Value? Value { get; set; }

        [ProtoConverter]
        public static VariableQueryArgumentSurrogate? Convert(VariableQueryArgument? source)
            => source is null
            ? null
            : new VariableQueryArgumentSurrogate
            {
                Type = source.Type,
                Value = Value.Wrap(source.Value),
            };

        [ProtoConverter]
        public static VariableQueryArgument? Convert(VariableQueryArgumentSurrogate? surrogate)
            => surrogate is null
            ? null
            : new VariableQueryArgument(surrogate.Value?.ObjectValue, surrogate.Type);
    }
}
