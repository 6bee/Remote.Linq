// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class LoopExpressionMapper : ProtoMapper<LoopExpression, Proto.LoopExpression>
{
    public static readonly LoopExpressionMapper Instance = new();

    public override LoopExpression FromProto(Proto.LoopExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Body = ExpressionMapper.Instance.FromProto(proto.Body, context),
            BreakLabel = LabelTargetMapper.Instance.FromProto(proto.BreakLabel, context),
            ContinueLabel = LabelTargetMapper.Instance.FromProto(proto.ContinueLabel, context),
        };

    public override Proto.LoopExpression ToProto(LoopExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Body = ExpressionMapper.Instance.ToProto(value.Body, context),
            BreakLabel = LabelTargetMapper.Instance.ToProto(value.BreakLabel!, context),
            ContinueLabel = LabelTargetMapper.Instance.ToProto(value.ContinueLabel!, context),
        };
}
