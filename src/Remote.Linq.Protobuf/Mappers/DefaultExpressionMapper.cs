// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class DefaultExpressionMapper : ProtoMapper<DefaultExpression, Proto.DefaultExpression>
{
    public static readonly DefaultExpressionMapper Instance = new();

    public override DefaultExpression FromProto(Proto.DefaultExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
        };

    public override Proto.DefaultExpression ToProto(DefaultExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
        };
}
