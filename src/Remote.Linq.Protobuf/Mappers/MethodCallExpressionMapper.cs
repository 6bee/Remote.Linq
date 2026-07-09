// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class MethodCallExpressionMapper : ProtoMapper<MethodCallExpression, Proto.MethodCallExpression>
{
    public static readonly MethodCallExpressionMapper Instance = new();

    public override MethodCallExpression FromProto(Proto.MethodCallExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Instance = ExpressionMapper.Instance.FromProto(proto.Instance, context),
            Method = MethodInfoMapper.Instance.FromProto(proto.Method, context),
            Arguments = ExpressionMapper.Instance.FromProto(proto.Arguments, context).ToListOrNull(),
        };

    public override Proto.MethodCallExpression ToProto(MethodCallExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.MethodCallExpression
        {
            Instance = value.Instance is null ? null : ExpressionMapper.Instance.ToProto(value.Instance, context),
            Method = MethodInfoMapper.Instance.ToProto(value.Method, context),
        };
        ExpressionMapper.Instance.ToProto(result.Arguments, value.Arguments, context);
        return result;
    }
}
