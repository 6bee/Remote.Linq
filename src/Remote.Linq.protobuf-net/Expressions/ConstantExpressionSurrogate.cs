// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.ProtoBuf.Expressions;

using Aqua.ProtoBuf;
using Aqua.TypeSystem;
using global::ProtoBuf;
using Remote.Linq.Expressions;

[ProtoContract(Name = nameof(ConstantExpression))]
public sealed class ConstantExpressionSurrogate
{
    [ProtoMember(1, IsRequired = true)]
    public TypeInfo Type { get; set; } = null!;

    [ProtoMember(2, DynamicType = true)]
    public Value? Value { get; set; }

    [ProtoConverter]
    public static ConstantExpressionSurrogate? Convert(ConstantExpression? source)
        => source is null
        ? null
        : new ConstantExpressionSurrogate
        {
            Type = source.Type,
            Value = Value.Wrap(source.Value),
        };

    [ProtoConverter]
    public static ConstantExpression? Convert(ConstantExpressionSurrogate? surrogate)
        => surrogate is null
        ? null
        : new ConstantExpression(surrogate.Value?.ObjectValue, surrogate.Type);
}