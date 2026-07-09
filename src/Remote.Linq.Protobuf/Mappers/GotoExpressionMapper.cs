// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Protobuf.Mappers;

using Aqua.Protobuf.Mappers;
using Remote.Linq.Expressions;
using Proto = Remote.Linq.Protobuf.Schema;

public sealed class GotoExpressionMapper : ProtoMapper<GotoExpression, Proto.GotoExpression>
{
    public static readonly GotoExpressionMapper Instance = new();

    public override GotoExpression FromProto(Proto.GotoExpression proto, ProtoContext context)
        => proto is null ? null! : new()
        {
            Kind = Map(proto.Kind),
            Target = LabelTargetMapper.Instance.FromProto(proto.Target, context),
            Type = TypeInfoMapper.Instance.FromProto(proto.Type, context),
            Value = ExpressionMapper.Instance.FromProto(proto.Value, context),
        };

    public override Proto.GotoExpression ToProto(GotoExpression value, ProtoContext context)
        => value is null ? null! : new()
        {
            Kind = Map(value.Kind),
            Target = LabelTargetMapper.Instance.ToProto(value.Target, context),
            Type = TypeInfoMapper.Instance.ToProto(value.Type!, context),
            Value = ExpressionMapper.Instance.ToProto(value.Value!, context),
        };

    private static GotoExpressionKind Map(Proto.GotoExpression.Types.GotoExpressionKind kind)
        => kind switch
        {
            Proto.GotoExpression.Types.GotoExpressionKind.Break => GotoExpressionKind.Break,
            Proto.GotoExpression.Types.GotoExpressionKind.Continue => GotoExpressionKind.Continue,
            Proto.GotoExpression.Types.GotoExpressionKind.Goto => GotoExpressionKind.Goto,
            Proto.GotoExpression.Types.GotoExpressionKind.Return => GotoExpressionKind.Return,
            _ => throw new ProtobufSerializationException($"Goto expression kind {kind} is not suported"),
        };

    private static Proto.GotoExpression.Types.GotoExpressionKind Map(GotoExpressionKind kind)
        => kind switch
        {
            GotoExpressionKind.Break => Proto.GotoExpression.Types.GotoExpressionKind.Break,
            GotoExpressionKind.Continue => Proto.GotoExpression.Types.GotoExpressionKind.Continue,
            GotoExpressionKind.Goto => Proto.GotoExpression.Types.GotoExpressionKind.Goto,
            GotoExpressionKind.Return => Proto.GotoExpression.Types.GotoExpressionKind.Return,
            _ => throw new ProtobufSerializationException($"Goto expression kind {kind} is not suported"),
        };
}
