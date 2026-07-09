// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Aqua.TypeExtensions;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class ExpressionMapper : ProtoMapper<Expression, Proto.Expression>
{
    public static readonly ExpressionMapper Instance = new();

    public override Expression FromProto(Proto.Expression proto, ProtoContext context)
        => proto is null ? null! : proto.KindCase switch
        {
            Proto.Expression.KindOneofCase.BinaryExpression => BinaryExpressionMapper.Instance.FromProto(proto.BinaryExpression, context),
            Proto.Expression.KindOneofCase.BlockExpression => BlockExpressionMapper.Instance.FromProto(proto.BlockExpression, context),
            Proto.Expression.KindOneofCase.ConditionalExpression => ConditionalExpressionMapper.Instance.FromProto(proto.ConditionalExpression, context),
            Proto.Expression.KindOneofCase.ConstantExpression => ConstantExpressionMapper.Instance.FromProto(proto.ConstantExpression, context),
            Proto.Expression.KindOneofCase.DefaultExpression => DefaultExpressionMapper.Instance.FromProto(proto.DefaultExpression, context),
            Proto.Expression.KindOneofCase.GotoExpression => GotoExpressionMapper.Instance.FromProto(proto.GotoExpression, context),
            Proto.Expression.KindOneofCase.InvokeExpression => InvokeExpressionMapper.Instance.FromProto(proto.InvokeExpression, context),
            Proto.Expression.KindOneofCase.LabelExpression => LabelExpressionMapper.Instance.FromProto(proto.LabelExpression, context),
            Proto.Expression.KindOneofCase.LambdaExpression => LambdaExpressionMapper.Instance.FromProto(proto.LambdaExpression, context),
            Proto.Expression.KindOneofCase.ListInitExpression => ListInitExpressionMapper.Instance.FromProto(proto.ListInitExpression, context),
            Proto.Expression.KindOneofCase.LoopExpression => LoopExpressionMapper.Instance.FromProto(proto.LoopExpression, context),
            Proto.Expression.KindOneofCase.MemberExpression => MemberExpressionMapper.Instance.FromProto(proto.MemberExpression, context),
            Proto.Expression.KindOneofCase.MemberInitExpression => MemberInitExpressionMapper.Instance.FromProto(proto.MemberInitExpression, context),
            Proto.Expression.KindOneofCase.MethodCallExpression => MethodCallExpressionMapper.Instance.FromProto(proto.MethodCallExpression, context),
            Proto.Expression.KindOneofCase.NewExpression => NewExpressionMapper.Instance.FromProto(proto.NewExpression, context),
            Proto.Expression.KindOneofCase.NewArrayExpression => NewArrayExpressionMapper.Instance.FromProto(proto.NewArrayExpression, context),
            Proto.Expression.KindOneofCase.ParameterExpression => ParameterExpressionMapper.Instance.FromProto(proto.ParameterExpression, context),
            Proto.Expression.KindOneofCase.SwitchExpression => SwitchExpressionMapper.Instance.FromProto(proto.SwitchExpression, context),
            Proto.Expression.KindOneofCase.TryExpression => TryExpressionMapper.Instance.FromProto(proto.TryExpression, context),
            Proto.Expression.KindOneofCase.TypeBinaryExpression => TypeBinaryExpressionMapper.Instance.FromProto(proto.TypeBinaryExpression, context),
            Proto.Expression.KindOneofCase.UnaryExpression => UnaryExpressionMapper.Instance.FromProto(proto.UnaryExpression, context),
            _ => throw SerializationException($"{proto.KindCase} is not supported"),
        };

    public override Proto.Expression ToProto(Expression value, ProtoContext context)
        => value switch
        {
            null => null!,
            BinaryExpression v => new() { BinaryExpression = BinaryExpressionMapper.Instance.ToProto(v, context) },
            BlockExpression v => new() { BlockExpression = BlockExpressionMapper.Instance.ToProto(v, context) },
            ConditionalExpression v => new() { ConditionalExpression = ConditionalExpressionMapper.Instance.ToProto(v, context) },
            ConstantExpression v => new() { ConstantExpression = ConstantExpressionMapper.Instance.ToProto(v, context) },
            DefaultExpression v => new() { DefaultExpression = DefaultExpressionMapper.Instance.ToProto(v, context) },
            GotoExpression v => new() { GotoExpression = GotoExpressionMapper.Instance.ToProto(v, context) },
            InvokeExpression v => new() { InvokeExpression = InvokeExpressionMapper.Instance.ToProto(v, context) },
            LabelExpression v => new() { LabelExpression = LabelExpressionMapper.Instance.ToProto(v, context) },
            LambdaExpression v => new() { LambdaExpression = LambdaExpressionMapper.Instance.ToProto(v, context) },
            ListInitExpression v => new() { ListInitExpression = ListInitExpressionMapper.Instance.ToProto(v, context) },
            LoopExpression v => new() { LoopExpression = LoopExpressionMapper.Instance.ToProto(v, context) },
            MemberExpression v => new() { MemberExpression = MemberExpressionMapper.Instance.ToProto(v, context) },
            MemberInitExpression v => new() { MemberInitExpression = MemberInitExpressionMapper.Instance.ToProto(v, context) },
            MethodCallExpression v => new() { MethodCallExpression = MethodCallExpressionMapper.Instance.ToProto(v, context) },
            NewExpression v => new() { NewExpression = NewExpressionMapper.Instance.ToProto(v, context) },
            NewArrayExpression v => new() { NewArrayExpression = NewArrayExpressionMapper.Instance.ToProto(v, context) },
            ParameterExpression v => new() { ParameterExpression = ParameterExpressionMapper.Instance.ToProto(v, context) },
            SwitchExpression v => new() { SwitchExpression = SwitchExpressionMapper.Instance.ToProto(v, context) },
            TryExpression v => new() { TryExpression = TryExpressionMapper.Instance.ToProto(v, context) },
            TypeBinaryExpression v => new() { TypeBinaryExpression = TypeBinaryExpressionMapper.Instance.ToProto(v, context) },
            UnaryExpression v => new() { UnaryExpression = UnaryExpressionMapper.Instance.ToProto(v, context) },
            _ => throw SerializationException($"{value?.GetType().GetFriendlyName()} is not supported"),
        };

    private static ProtobufSerializationException SerializationException(string message) => new(message);
}
