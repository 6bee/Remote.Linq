// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class CatchBlockMapper : ProtoMapper<CatchBlock, Proto.CatchBlock>
{
    public static readonly CatchBlockMapper Instance = new();

    public override CatchBlock FromProto(Proto.CatchBlock proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Variable = ParameterExpressionMapper.Instance.FromProto(proto.Variable, context),
            Filter = ExpressionMapper.Instance.FromProto(proto.Filter, context),
            Body = ExpressionMapper.Instance.FromProto(proto.Body, context),
            Test = TypeInfoMapper.Instance.FromProto(proto.Test, context),
        };

    public override Proto.CatchBlock ToProto(CatchBlock value, ProtoContext context)
        => value is null ? null! : new()
        {
            Variable = ParameterExpressionMapper.Instance.ToProto(value.Variable!, context),
            Filter = ExpressionMapper.Instance.ToProto(value.Filter!, context),
            Body = ExpressionMapper.Instance.ToProto(value.Body, context),
            Test = TypeInfoMapper.Instance.ToProto(value.Test, context),
        };
}
