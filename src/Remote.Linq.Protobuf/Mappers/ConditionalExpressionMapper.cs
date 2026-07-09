// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ConditionalExpressionMapper : ProtoMapper<ConditionalExpression, Proto.ConditionalExpression>
{
    public static readonly ConditionalExpressionMapper Instance = new();

    public override ConditionalExpression FromProto(Proto.ConditionalExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Test = ExpressionMapper.Instance.FromProto(proto.Test, context),
            IfTrue = ExpressionMapper.Instance.FromProto(proto.IfTrue, context),
            IfFalse = ExpressionMapper.Instance.FromProto(proto.IfFalse, context),
        };

    public override Proto.ConditionalExpression ToProto(ConditionalExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Test = ExpressionMapper.Instance.ToProto(value.Test, context),
            IfTrue = ExpressionMapper.Instance.ToProto(value.IfTrue, context),
            IfFalse = ExpressionMapper.Instance.ToProto(value.IfFalse, context),
        };
}
