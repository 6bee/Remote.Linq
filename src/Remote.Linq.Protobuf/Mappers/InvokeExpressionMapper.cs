// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class InvokeExpressionMapper : ProtoMapper<InvokeExpression, Proto.InvokeExpression>
{
    public static readonly InvokeExpressionMapper Instance = new();

    public override InvokeExpression FromProto(Proto.InvokeExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Expression = ExpressionMapper.Instance.FromProto(proto.Expression, context),
            Arguments = ExpressionMapper.Instance.FromProto(proto.Arguments, context).ToListOrNull(),
        };

    public override Proto.InvokeExpression ToProto(InvokeExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.InvokeExpression
        {
            Expression = ExpressionMapper.Instance.ToProto(value.Expression, context),
        };
        ExpressionMapper.Instance.ToProto(result.Arguments, value.Arguments, context);
        return result;
    }
}
