// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.DynamicQuery;

using Aqua.ProtoBuf;
using Aqua.TypeSystem;
using global::ProtoBuf;
using Remote.Linq.DynamicQuery;
using System.Collections;

[ProtoContract(Name = nameof(VariableQueryArgumentList))]
public class VariableQueryArgumentListSurrogate
{
    [ProtoMember(1, IsRequired = true)]
    public TypeInfo ElementType { get; set; } = null!;

    [ProtoMember(2, DynamicType = true)]
    public Values Values { get; set; } = null!;

    [ProtoConverter]
    public static VariableQueryArgumentListSurrogate? Convert(VariableQueryArgumentList? source)
        => source is null
        ? null
        : new VariableQueryArgumentListSurrogate
        {
            ElementType = source.ElementType,
            Values = Values.Wrap(source.Values, source.ElementType.ToType()),
        };

    [ProtoConverter]
    public static VariableQueryArgumentList? Convert(VariableQueryArgumentListSurrogate? surrogate)
        => surrogate?.Values.ObjectValue is IEnumerable enumerable
        ? new VariableQueryArgumentList(enumerable, surrogate.ElementType)
        : null;
}