// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ConstantExpressionMapper : ProtoMapper<ConstantExpression, Proto.ConstantExpression>
{
    public static readonly ConstantExpressionMapper Instance = new();

    public override ConstantExpression FromProto(Proto.ConstantExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Value = ConstantValueMapper.Instance.FromProto(proto.Value, context),
        };

    public override Proto.ConstantExpression ToProto(ConstantExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
            Value = ConstantValueMapper.Instance.ToProto(value.Value, context),
        };
}
