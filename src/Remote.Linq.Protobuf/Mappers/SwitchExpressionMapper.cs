// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class SwitchExpressionMapper : ProtoMapper<SwitchExpression, Proto.SwitchExpression>
{
    public static readonly SwitchExpressionMapper Instance = new();

    public override SwitchExpression FromProto(Proto.SwitchExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            SwitchValue = ExpressionMapper.Instance.FromProto(proto.SwitchValue, context),
            Comparison = MethodInfoMapper.Instance.FromProto(proto.Comparison, context),
            DefaultExpression = ExpressionMapper.Instance.FromProto(proto.DefaultExpression, context),
            Cases = SwitchCaseMapper.Instance.FromProto(proto.Cases, context).ToListOrNull(),
        };

    public override Proto.SwitchExpression ToProto(SwitchExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.SwitchExpression
        {
            SwitchValue = ExpressionMapper.Instance.ToProto(value.SwitchValue, context),
            Comparison = MethodInfoMapper.Instance.ToProto(value.Comparison!, context),
            DefaultExpression = ExpressionMapper.Instance.ToProto(value.DefaultExpression!, context),
        };
        SwitchCaseMapper.Instance.ToProto(result.Cases, value.Cases, context);
        return result;
    }
}
