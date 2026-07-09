// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class NewArrayExpressionMapper : ProtoMapper<NewArrayExpression, Proto.NewArrayExpression>
{
    public static readonly NewArrayExpressionMapper Instance = new();

    public override NewArrayExpression FromProto(Proto.NewArrayExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            NewArrayType = Map(proto.NewArrayType),
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Expressions = [.. ExpressionMapper.Instance.FromProto(proto.Expressions, context)],
        };

    public override Proto.NewArrayExpression ToProto(NewArrayExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.NewArrayExpression
        {
            NewArrayType = Map(value.NewArrayType),
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
        };
        ExpressionMapper.Instance.ToProto(result.Expressions, value.Expressions, context);
        return result;
    }

    private NewArrayType Map(Proto.NewArrayExpression.Types.NewArrayType newArrayType)
        => newArrayType switch
        {
            Proto.NewArrayExpression.Types.NewArrayType.NewArrayBounds => NewArrayType.NewArrayBounds,
            Proto.NewArrayExpression.Types.NewArrayType.NewArrayInit => NewArrayType.NewArrayInit,
            _ => throw new ProtobufSerializationException($"New array type {newArrayType} is not suported"),
        };

    private Proto.NewArrayExpression.Types.NewArrayType Map(NewArrayType newArrayType)
        => newArrayType switch
        {
            NewArrayType.NewArrayBounds => Proto.NewArrayExpression.Types.NewArrayType.NewArrayBounds,
            NewArrayType.NewArrayInit => Proto.NewArrayExpression.Types.NewArrayType.NewArrayInit,
            _ => throw new ProtobufSerializationException($"New array type {newArrayType} is not suported"),
        };
}
