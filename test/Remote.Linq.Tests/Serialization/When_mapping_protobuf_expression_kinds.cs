// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.Protobuf;
using Remote.Linq.Protobuf;
using Remote.Linq.Protobuf.Mappers;
using Proto = Remote.Linq.Protobuf.Schema;
using RemoteLinq = Remote.Linq.Expressions;

/// <summary>
/// Verifies the protobuf <see cref="ExpressionMapper"/> oneof kind mapping for all remote expression kinds.
/// </summary>
public class When_mapping_protobuf_expression_kinds
{
    private static readonly ProtoContext ReadContext = ProtoContext.ForRead(ProtoOptions.WithRemoteLinqTypesOptimized);

    private static readonly ProtoContext WriteContext = ProtoContext.ForWrite(ProtoOptions.WithRemoteLinqTypesOptimized);

    [Theory]
    [MemberData(nameof(AllExpressionKinds))]
    public void Should_map_expression_to_proto_with_matching_oneof_case(RemoteLinq.ExpressionType nodeType, Proto.Expression.KindOneofCase expectedKindCase)
    {
        var expression = UnionTagExpressionFactory.Build(nodeType);

        var proto = ExpressionMapper.Instance.ToProto(expression, WriteContext);

        proto.KindCase.ShouldBe(expectedKindCase);
    }

    [Theory]
    [MemberData(nameof(AllExpressionKinds))]
    public void Should_map_proto_oneof_case_back_to_expression_kind(RemoteLinq.ExpressionType nodeType, Proto.Expression.KindOneofCase expectedKindCase)
    {
        var expression = UnionTagExpressionFactory.Build(nodeType);
        var proto = ExpressionMapper.Instance.ToProto(expression, WriteContext);
        proto.KindCase.ShouldBe(expectedKindCase);

        var clone = ExpressionMapper.Instance.FromProto(proto, ReadContext);

        clone.NodeType.ShouldBe(nodeType);
        clone.ShouldBeOfType(expression.GetType());
    }

    [Fact]
    public void Should_map_null_expression_to_null_proto()
        => ExpressionMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_proto_to_null_expression()
        => ExpressionMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_proto_expression_without_selected_oneof_case()
    {
        var act = () => ExpressionMapper.Instance.FromProto(new Proto.Expression(), ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("None is not supported");
    }

    public static IEnumerable<object[]> AllExpressionKinds =>
    [
        [RemoteLinq.ExpressionType.Binary, Proto.Expression.KindOneofCase.BinaryExpression],
        [RemoteLinq.ExpressionType.Block, Proto.Expression.KindOneofCase.BlockExpression],
        [RemoteLinq.ExpressionType.Conditional, Proto.Expression.KindOneofCase.ConditionalExpression],
        [RemoteLinq.ExpressionType.Constant, Proto.Expression.KindOneofCase.ConstantExpression],
        [RemoteLinq.ExpressionType.Default, Proto.Expression.KindOneofCase.DefaultExpression],
        [RemoteLinq.ExpressionType.Goto, Proto.Expression.KindOneofCase.GotoExpression],
        [RemoteLinq.ExpressionType.Invoke, Proto.Expression.KindOneofCase.InvokeExpression],
        [RemoteLinq.ExpressionType.Label, Proto.Expression.KindOneofCase.LabelExpression],
        [RemoteLinq.ExpressionType.Lambda, Proto.Expression.KindOneofCase.LambdaExpression],
        [RemoteLinq.ExpressionType.ListInit, Proto.Expression.KindOneofCase.ListInitExpression],
        [RemoteLinq.ExpressionType.Loop, Proto.Expression.KindOneofCase.LoopExpression],
        [RemoteLinq.ExpressionType.MemberAccess, Proto.Expression.KindOneofCase.MemberExpression],
        [RemoteLinq.ExpressionType.MemberInit, Proto.Expression.KindOneofCase.MemberInitExpression],
        [RemoteLinq.ExpressionType.Call, Proto.Expression.KindOneofCase.MethodCallExpression],
        [RemoteLinq.ExpressionType.New, Proto.Expression.KindOneofCase.NewExpression],
        [RemoteLinq.ExpressionType.NewArray, Proto.Expression.KindOneofCase.NewArrayExpression],
        [RemoteLinq.ExpressionType.Parameter, Proto.Expression.KindOneofCase.ParameterExpression],
        [RemoteLinq.ExpressionType.Switch, Proto.Expression.KindOneofCase.SwitchExpression],
        [RemoteLinq.ExpressionType.Try, Proto.Expression.KindOneofCase.TryExpression],
        [RemoteLinq.ExpressionType.TypeIs, Proto.Expression.KindOneofCase.TypeBinaryExpression],
        [RemoteLinq.ExpressionType.Unary, Proto.Expression.KindOneofCase.UnaryExpression],
    ];
}
