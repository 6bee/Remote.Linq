// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class LambdaExpressionMapper : ProtoMapper<LambdaExpression, Proto.LambdaExpression>
{
    public static readonly LambdaExpressionMapper Instance = new();

    public override LambdaExpression FromProto(Proto.LambdaExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Expression = ExpressionMapper.Instance.FromProto(proto.Expression, context),
            Parameters = ParameterExpressionMapper.Instance.FromProto(proto.Parameters, context).ToListOrNull(),
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
        };

    public override Proto.LambdaExpression ToProto(LambdaExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.LambdaExpression
        {
            Expression = ExpressionMapper.Instance.ToProto(value.Expression, context),
            Type = TypeInfoMapper.Instance.ToProto(value.Type!, context),
        };
        ParameterExpressionMapper.Instance.ToProto(result.Parameters, value.Parameters, context);
        return result;
    }
}
