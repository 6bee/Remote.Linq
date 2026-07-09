// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class LabelExpressionMapper : ProtoMapper<LabelExpression, Proto.LabelExpression>
{
    public static readonly LabelExpressionMapper Instance = new();

    public override LabelExpression FromProto(Proto.LabelExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Target = LabelTargetMapper.Instance.FromProto(proto.Target, context),
            DefaultValue = ExpressionMapper.Instance.FromProto(proto.DefaultValue, context),
        };

    public override Proto.LabelExpression ToProto(LabelExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Target = LabelTargetMapper.Instance.ToProto(value.Target, context),
            DefaultValue = ExpressionMapper.Instance.ToProto(value.DefaultValue!, context),
        };
}
