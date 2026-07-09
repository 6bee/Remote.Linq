// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.DynamicQuery;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ConstantQueryArgumentMapper : ProtoMapper<ConstantQueryArgument, Proto.ConstantQueryArgument>
{
    public static readonly ConstantQueryArgumentMapper Instance = new();

    public override ConstantQueryArgument FromProto(Proto.ConstantQueryArgument proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Value = DynamicObjectMapper.Instance.FromProto(proto.Value, context),
        };

    public override Proto.ConstantQueryArgument ToProto(ConstantQueryArgument value, ProtoContext context)
        => value is null ? null! : new()
        {
            Value = DynamicObjectMapper.Instance.ToProto(value.Value, context),
        };
}
