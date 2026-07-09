// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class LabelTargetMapper : ProtoMapper<LabelTarget, Proto.LabelTarget>
{
    public static readonly LabelTargetMapper Instance = new();

    public override LabelTarget FromProto(Proto.LabelTarget proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Name = proto.Name,
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            InstanceId = proto.InstanceId,
        };

    public override Proto.LabelTarget ToProto(LabelTarget value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.LabelTarget
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type!, context),
            InstanceId = value.InstanceId,
        };

        if (value.Name?.Length > 0)
        {
            result.Name = value.Name;
        }

        return result;
    }
}
