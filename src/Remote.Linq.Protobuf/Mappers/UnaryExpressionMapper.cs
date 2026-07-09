// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class UnaryExpressionMapper : ProtoMapper<UnaryExpression, Proto.UnaryExpression>
{
    public static readonly UnaryExpressionMapper Instance = new();

    public override UnaryExpression FromProto(Proto.UnaryExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            UnaryOperator = Map(proto.UnaryOperator),
            Operand = ExpressionMapper.Instance.FromProto(proto.Operand, context),
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Method = MethodInfoMapper.Instance.FromProto(proto.Method, context),
        };

    public override Proto.UnaryExpression ToProto(UnaryExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        return new()
        {
            UnaryOperator = Map(value.UnaryOperator),
            Operand = ExpressionMapper.Instance.ToProto(value.Operand, context),
            Type = TypeInfoMapper.Instance.ToProto(value.Type, context),
            Method = value.Method is null ? null : MethodInfoMapper.Instance.ToProto(value.Method, context),
        };
    }

    private static UnaryOperator Map(Proto.UnaryExpression.Types.UnaryOperator value)
        => value switch
        {
            Proto.UnaryExpression.Types.UnaryOperator.ArrayLength => UnaryOperator.ArrayLength,
            Proto.UnaryExpression.Types.UnaryOperator.Convert => UnaryOperator.Convert,
            Proto.UnaryExpression.Types.UnaryOperator.ConvertChecked => UnaryOperator.ConvertChecked,
            Proto.UnaryExpression.Types.UnaryOperator.Negate => UnaryOperator.Negate,
            Proto.UnaryExpression.Types.UnaryOperator.NegateChecked => UnaryOperator.NegateChecked,
            Proto.UnaryExpression.Types.UnaryOperator.Not => UnaryOperator.Not,
            Proto.UnaryExpression.Types.UnaryOperator.Quote => UnaryOperator.Quote,
            Proto.UnaryExpression.Types.UnaryOperator.TypeAs => UnaryOperator.TypeAs,
            Proto.UnaryExpression.Types.UnaryOperator.UnaryPlus => UnaryOperator.UnaryPlus,
            Proto.UnaryExpression.Types.UnaryOperator.PreDecrementAssign => UnaryOperator.PreDecrementAssign,
            Proto.UnaryExpression.Types.UnaryOperator.PreIncrementAssign => UnaryOperator.PreIncrementAssign,
            Proto.UnaryExpression.Types.UnaryOperator.PostDecrementAssign => UnaryOperator.PostDecrementAssign,
            Proto.UnaryExpression.Types.UnaryOperator.PostIncrementAssign => UnaryOperator.PostIncrementAssign,
            Proto.UnaryExpression.Types.UnaryOperator.Throw => UnaryOperator.Throw,
            _ => throw new ProtobufSerializationException($"Unary operator {value} is not suported"),
        };

    private static Proto.UnaryExpression.Types.UnaryOperator Map(UnaryOperator value)
        => value switch
        {
            UnaryOperator.ArrayLength => Proto.UnaryExpression.Types.UnaryOperator.ArrayLength,
            UnaryOperator.Convert => Proto.UnaryExpression.Types.UnaryOperator.Convert,
            UnaryOperator.ConvertChecked => Proto.UnaryExpression.Types.UnaryOperator.ConvertChecked,
            UnaryOperator.Negate => Proto.UnaryExpression.Types.UnaryOperator.Negate,
            UnaryOperator.NegateChecked => Proto.UnaryExpression.Types.UnaryOperator.NegateChecked,
            UnaryOperator.Not => Proto.UnaryExpression.Types.UnaryOperator.Not,
            UnaryOperator.Quote => Proto.UnaryExpression.Types.UnaryOperator.Quote,
            UnaryOperator.TypeAs => Proto.UnaryExpression.Types.UnaryOperator.TypeAs,
            UnaryOperator.UnaryPlus => Proto.UnaryExpression.Types.UnaryOperator.UnaryPlus,
            UnaryOperator.PreDecrementAssign => Proto.UnaryExpression.Types.UnaryOperator.PreDecrementAssign,
            UnaryOperator.PreIncrementAssign => Proto.UnaryExpression.Types.UnaryOperator.PreIncrementAssign,
            UnaryOperator.PostDecrementAssign => Proto.UnaryExpression.Types.UnaryOperator.PostDecrementAssign,
            UnaryOperator.PostIncrementAssign => Proto.UnaryExpression.Types.UnaryOperator.PostIncrementAssign,
            UnaryOperator.Throw => Proto.UnaryExpression.Types.UnaryOperator.Throw,
            _ => throw new ProtobufSerializationException($"Unary operator {value} is not suported"),
        };
}
