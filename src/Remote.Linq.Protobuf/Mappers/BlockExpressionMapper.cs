// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class BlockExpressionMapper : ProtoMapper<BlockExpression, Proto.BlockExpression>
{
    public static readonly BlockExpressionMapper Instance = new();

    public override BlockExpression FromProto(Proto.BlockExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Variables = ParameterExpressionMapper.Instance.FromProto(proto.Variables, context).ToListOrNull(),
            Expressions = ExpressionMapper.Instance.FromProto(proto.Expressions, context).ToListOrNull(),
        };

    public override Proto.BlockExpression ToProto(BlockExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.BlockExpression
        {
            Type = TypeInfoMapper.Instance.ToProto(value.Type!, context),
        };
        ParameterExpressionMapper.Instance.ToProto(result.Variables, value.Variables, context);
        ExpressionMapper.Instance.ToProto(result.Expressions, value.Expressions, context);
        return result;
    }
}
