// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class TryExpressionMapper : ProtoMapper<TryExpression, Proto.TryExpression>
{
    public static readonly TryExpressionMapper Instance = new();

    public override TryExpression FromProto(Proto.TryExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Body = ExpressionMapper.Instance.FromProto(proto.Body, context),
            Handlers = CatchBlockMapper.Instance.FromProto(proto.Handlers, context).ToListOrNull(),
            Finally = ExpressionMapper.Instance.FromProto(proto.Finally, context),
            Fault = ExpressionMapper.Instance.FromProto(proto.Fault, context),
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
        };

    public override Proto.TryExpression ToProto(TryExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.TryExpression
        {
            Body = ExpressionMapper.Instance.ToProto(value.Body, context),
            Finally = ExpressionMapper.Instance.ToProto(value.Finally!, context),
            Fault = ExpressionMapper.Instance.ToProto(value.Fault!, context),
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
        };
        CatchBlockMapper.Instance.ToProto(result.Handlers, value.Handlers, context);
        return result;
    }
}
