// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.DynamicQuery;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class VariableQueryArgumentMapper : ProtoMapper<VariableQueryArgument, Proto.VariableQueryArgument>
{
    public static readonly VariableQueryArgumentMapper Instance = new();

    public override VariableQueryArgument FromProto(Proto.VariableQueryArgument proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Value = ValueMapper.Instance.FromProto(proto.Value, context),
        };

    public override Proto.VariableQueryArgument ToProto(VariableQueryArgument value, ProtoContext context)
        => value is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
            Value = ValueMapper.Instance.ToProto(value.Value, context),
        };
}
