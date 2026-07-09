// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class BinaryExpressionMapper : ProtoMapper<BinaryExpression, Proto.BinaryExpression>
{
    public static readonly BinaryExpressionMapper Instance = new();

    public override BinaryExpression FromProto(Proto.BinaryExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            BinaryOperator = Map(proto.BinaryOperator),
            LeftOperand = ExpressionMapper.Instance.FromProto(proto.LeftOperand, context),
            RightOperand = ExpressionMapper.Instance.FromProto(proto.RightOperand, context),
            IsLiftedToNull = proto.HasIsLiftedToNull ? proto.IsLiftedToNull : default,
            Method = MethodInfoMapper.Instance.FromProto(proto.Method, context),
            Conversion = LambdaExpressionMapper.Instance.FromProto(proto.Conversion, context),
        };

    public override Proto.BinaryExpression ToProto(BinaryExpression value, ProtoContext context)
    {
        if (value is null)
        {
            return null!;
        }

        var result = new Proto.BinaryExpression
        {
            BinaryOperator = Map(value.BinaryOperator),
            LeftOperand = ExpressionMapper.Instance.ToProto(value.LeftOperand, context),
            RightOperand = ExpressionMapper.Instance.ToProto(value.RightOperand, context),
            Method = value.Method is null ? null : MethodInfoMapper.Instance.ToProto(value.Method, context),
            Conversion = LambdaExpressionMapper.Instance.ToProto(value.Conversion!, context),
        };

        if (value.IsLiftedToNull)
        {
            result.IsLiftedToNull = true;
        }

        return result;
    }

    private static BinaryOperator Map(Proto.BinaryExpression.Types.BinaryOperator value)
        => value switch
        {
            // Binary Arithmetic Operations
            Proto.BinaryExpression.Types.BinaryOperator.Add => BinaryOperator.Add,
            Proto.BinaryExpression.Types.BinaryOperator.AddChecked => BinaryOperator.AddChecked,
            Proto.BinaryExpression.Types.BinaryOperator.Divide => BinaryOperator.Divide,
            Proto.BinaryExpression.Types.BinaryOperator.Modulo => BinaryOperator.Modulo,
            Proto.BinaryExpression.Types.BinaryOperator.Multiply => BinaryOperator.Multiply,
            Proto.BinaryExpression.Types.BinaryOperator.MultiplyChecked => BinaryOperator.MultiplyChecked,
            Proto.BinaryExpression.Types.BinaryOperator.Power => BinaryOperator.Power,
            Proto.BinaryExpression.Types.BinaryOperator.Subtract => BinaryOperator.Subtract,
            Proto.BinaryExpression.Types.BinaryOperator.SubtractChecked => BinaryOperator.SubtractChecked,

            // Bitwise Operations
            Proto.BinaryExpression.Types.BinaryOperator.And => BinaryOperator.And,
            Proto.BinaryExpression.Types.BinaryOperator.Or => BinaryOperator.Or,
            Proto.BinaryExpression.Types.BinaryOperator.ExclusiveOr => BinaryOperator.ExclusiveOr,

            // Shift Operations
            Proto.BinaryExpression.Types.BinaryOperator.LeftShift => BinaryOperator.LeftShift,
            Proto.BinaryExpression.Types.BinaryOperator.RightShift => BinaryOperator.RightShift,

            // Conditional Boolean Operations
            Proto.BinaryExpression.Types.BinaryOperator.AndAlso => BinaryOperator.AndAlso,
            Proto.BinaryExpression.Types.BinaryOperator.OrElse => BinaryOperator.OrElse,

            // Comparison Operations
            Proto.BinaryExpression.Types.BinaryOperator.Equal => BinaryOperator.Equal,
            Proto.BinaryExpression.Types.BinaryOperator.NotEqual => BinaryOperator.NotEqual,
            Proto.BinaryExpression.Types.BinaryOperator.GreaterThanOrEqual => BinaryOperator.GreaterThanOrEqual,
            Proto.BinaryExpression.Types.BinaryOperator.GreaterThan => BinaryOperator.GreaterThan,
            Proto.BinaryExpression.Types.BinaryOperator.LessThan => BinaryOperator.LessThan,
            Proto.BinaryExpression.Types.BinaryOperator.LessThanOrEqual => BinaryOperator.LessThanOrEqual,

            // Coalescing Operations
            Proto.BinaryExpression.Types.BinaryOperator.Coalesce => BinaryOperator.Coalesce,

            // Array Indexing Operations
            Proto.BinaryExpression.Types.BinaryOperator.ArrayIndex => BinaryOperator.ArrayIndex,

            // Assignment Operations
            Proto.BinaryExpression.Types.BinaryOperator.AddAssign => BinaryOperator.AddAssign,
            Proto.BinaryExpression.Types.BinaryOperator.AddAssignChecked => BinaryOperator.AddAssignChecked,
            Proto.BinaryExpression.Types.BinaryOperator.AndAssign => BinaryOperator.AndAssign,
            Proto.BinaryExpression.Types.BinaryOperator.Assign => BinaryOperator.Assign,
            Proto.BinaryExpression.Types.BinaryOperator.DivideAssign => BinaryOperator.DivideAssign,
            Proto.BinaryExpression.Types.BinaryOperator.ExclusiveOrAssign => BinaryOperator.ExclusiveOrAssign,
            Proto.BinaryExpression.Types.BinaryOperator.LeftShiftAssign => BinaryOperator.LeftShiftAssign,
            Proto.BinaryExpression.Types.BinaryOperator.ModuloAssign => BinaryOperator.ModuloAssign,
            Proto.BinaryExpression.Types.BinaryOperator.MultiplyAssign => BinaryOperator.MultiplyAssign,
            Proto.BinaryExpression.Types.BinaryOperator.MultiplyAssignChecked => BinaryOperator.MultiplyAssignChecked,
            Proto.BinaryExpression.Types.BinaryOperator.OrAssign => BinaryOperator.OrAssign,
            Proto.BinaryExpression.Types.BinaryOperator.PowerAssign => BinaryOperator.PowerAssign,
            Proto.BinaryExpression.Types.BinaryOperator.RightShiftAssign => BinaryOperator.RightShiftAssign,
            Proto.BinaryExpression.Types.BinaryOperator.SubtractAssign => BinaryOperator.SubtractAssign,
            Proto.BinaryExpression.Types.BinaryOperator.SubtractAssignChecked => BinaryOperator.SubtractAssignChecked,

            _ => throw new ProtobufSerializationException($"Binary operator {value} is not suported"),
        };

    private static Proto.BinaryExpression.Types.BinaryOperator Map(BinaryOperator value)
        => value switch
        {
            // Binary Arithmetic Operations
            BinaryOperator.Add => Proto.BinaryExpression.Types.BinaryOperator.Add,
            BinaryOperator.AddChecked => Proto.BinaryExpression.Types.BinaryOperator.AddChecked,
            BinaryOperator.Divide => Proto.BinaryExpression.Types.BinaryOperator.Divide,
            BinaryOperator.Modulo => Proto.BinaryExpression.Types.BinaryOperator.Modulo,
            BinaryOperator.Multiply => Proto.BinaryExpression.Types.BinaryOperator.Multiply,
            BinaryOperator.MultiplyChecked => Proto.BinaryExpression.Types.BinaryOperator.MultiplyChecked,
            BinaryOperator.Power => Proto.BinaryExpression.Types.BinaryOperator.Power,
            BinaryOperator.Subtract => Proto.BinaryExpression.Types.BinaryOperator.Subtract,
            BinaryOperator.SubtractChecked => Proto.BinaryExpression.Types.BinaryOperator.SubtractChecked,

            // Bitwise Operations
            BinaryOperator.And => Proto.BinaryExpression.Types.BinaryOperator.And,
            BinaryOperator.Or => Proto.BinaryExpression.Types.BinaryOperator.Or,
            BinaryOperator.ExclusiveOr => Proto.BinaryExpression.Types.BinaryOperator.ExclusiveOr,

            // Shift Operations
            BinaryOperator.LeftShift => Proto.BinaryExpression.Types.BinaryOperator.LeftShift,
            BinaryOperator.RightShift => Proto.BinaryExpression.Types.BinaryOperator.RightShift,

            // Conditional Boolean Operations
            BinaryOperator.AndAlso => Proto.BinaryExpression.Types.BinaryOperator.AndAlso,
            BinaryOperator.OrElse => Proto.BinaryExpression.Types.BinaryOperator.OrElse,

            // Comparison Operations
            BinaryOperator.Equal => Proto.BinaryExpression.Types.BinaryOperator.Equal,
            BinaryOperator.NotEqual => Proto.BinaryExpression.Types.BinaryOperator.NotEqual,
            BinaryOperator.GreaterThanOrEqual => Proto.BinaryExpression.Types.BinaryOperator.GreaterThanOrEqual,
            BinaryOperator.GreaterThan => Proto.BinaryExpression.Types.BinaryOperator.GreaterThan,
            BinaryOperator.LessThan => Proto.BinaryExpression.Types.BinaryOperator.LessThan,
            BinaryOperator.LessThanOrEqual => Proto.BinaryExpression.Types.BinaryOperator.LessThanOrEqual,

            // Coalescing Operations
            BinaryOperator.Coalesce => Proto.BinaryExpression.Types.BinaryOperator.Coalesce,

            // Array Indexing Operations
            BinaryOperator.ArrayIndex => Proto.BinaryExpression.Types.BinaryOperator.ArrayIndex,

            // Assignment Operations
            BinaryOperator.AddAssign => Proto.BinaryExpression.Types.BinaryOperator.AddAssign,
            BinaryOperator.AddAssignChecked => Proto.BinaryExpression.Types.BinaryOperator.AddAssignChecked,
            BinaryOperator.AndAssign => Proto.BinaryExpression.Types.BinaryOperator.AndAssign,
            BinaryOperator.Assign => Proto.BinaryExpression.Types.BinaryOperator.Assign,
            BinaryOperator.DivideAssign => Proto.BinaryExpression.Types.BinaryOperator.DivideAssign,
            BinaryOperator.ExclusiveOrAssign => Proto.BinaryExpression.Types.BinaryOperator.ExclusiveOrAssign,
            BinaryOperator.LeftShiftAssign => Proto.BinaryExpression.Types.BinaryOperator.LeftShiftAssign,
            BinaryOperator.ModuloAssign => Proto.BinaryExpression.Types.BinaryOperator.ModuloAssign,
            BinaryOperator.MultiplyAssign => Proto.BinaryExpression.Types.BinaryOperator.MultiplyAssign,
            BinaryOperator.MultiplyAssignChecked => Proto.BinaryExpression.Types.BinaryOperator.MultiplyAssignChecked,
            BinaryOperator.OrAssign => Proto.BinaryExpression.Types.BinaryOperator.OrAssign,
            BinaryOperator.PowerAssign => Proto.BinaryExpression.Types.BinaryOperator.PowerAssign,
            BinaryOperator.RightShiftAssign => Proto.BinaryExpression.Types.BinaryOperator.RightShiftAssign,
            BinaryOperator.SubtractAssign => Proto.BinaryExpression.Types.BinaryOperator.SubtractAssign,
            BinaryOperator.SubtractAssignChecked => Proto.BinaryExpression.Types.BinaryOperator.SubtractAssignChecked,

            _ => throw new ProtobufSerializationException($"Binary operator {value} is not suported"),
        };
}
