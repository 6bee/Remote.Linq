// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ParameterExpressionMapper : ProtoMapper<ParameterExpression, Proto.ParameterExpression>
{
    public static readonly ParameterExpressionMapper Instance = new();

    public override ParameterExpression FromProto(Proto.ParameterExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            ParameterName = proto.ParameterName,
            ParameterType = TypeInfoMapper.Instance.FromProto(proto.ParameterType, context),
            InstanceId = proto.HasInstanceId ? proto.InstanceId : default,
        };

    public override Proto.ParameterExpression ToProto(ParameterExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.ParameterExpression
        {
            ParameterType = TypeInfoMapper.Instance.ToProto(value.ParameterType, context),
        };

        if (value.ParameterName?.Length > 0)
        {
            result.ParameterName = value.ParameterName;
        }

        if (value.InstanceId != default)
        {
            result.InstanceId = value.InstanceId;
        }

        return result;
    }
}
