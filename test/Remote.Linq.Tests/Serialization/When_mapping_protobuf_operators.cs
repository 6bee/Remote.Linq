// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.Dynamic;
using Aqua.Protobuf;
using Remote.Linq.DynamicQuery;
using Remote.Linq.Protobuf;
using Remote.Linq.Protobuf.Mappers;
using Proto = Remote.Linq.Protobuf.Schema;
using RemoteLinq = Remote.Linq.Expressions;
using VQArg = global::Remote.Linq.DynamicQuery.VariableQueryArgument;

/// <summary>
/// Verifies the protobuf operator and secondary tagged-union mappers (<c>BinaryExpression</c>, <c>UnaryExpression</c>, <c>GotoExpression</c>, <c>NewArrayExpression</c>, <c>MemberBinding</c> and <c>ConstantValue</c>).
/// </summary>
public class When_mapping_protobuf_operators
{
    private static readonly ProtoContext ReadContext = ProtoContext.ForRead(ProtoOptions.WithRemoteLinqTypesOptimized);

    private static readonly ProtoContext WriteContext = ProtoContext.ForWrite(ProtoOptions.WithRemoteLinqTypesOptimized);

    [Theory]
    [MemberData(nameof(AllBinaryOperators))]
    public void Should_map_binary_operator_both_ways(RemoteLinq.BinaryOperator op, Proto.BinaryExpression.Types.BinaryOperator protoOp)
    {
        var remote = new RemoteLinq.BinaryExpression(op, new RemoteLinq.ConstantExpression(1, typeof(int)), new RemoteLinq.ConstantExpression(2, typeof(int)));

        var proto = BinaryExpressionMapper.Instance.ToProto(remote, WriteContext);
        proto.BinaryOperator.ShouldBe(protoOp);

        var clone = BinaryExpressionMapper.Instance.FromProto(new Proto.BinaryExpression { BinaryOperator = protoOp }, ReadContext);
        clone.BinaryOperator.ShouldBe(op);
    }

    [Fact]
    public void Should_map_null_binary_expression_to_null_proto()
        => BinaryExpressionMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_binary_proto_to_null_expression()
        => BinaryExpressionMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_unknown_binary_operator_in_proto()
    {
        var act = () => BinaryExpressionMapper.Instance.FromProto(new Proto.BinaryExpression { BinaryOperator = (Proto.BinaryExpression.Types.BinaryOperator)int.MaxValue }, ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("Binary operator 2147483647 is not suported");
    }

    [Fact]
    public void Should_throw_for_unknown_binary_operator_in_remote_expression()
    {
        var remote = new RemoteLinq.BinaryExpression((RemoteLinq.BinaryOperator)int.MaxValue, new RemoteLinq.ConstantExpression(1, typeof(int)), new RemoteLinq.ConstantExpression(2, typeof(int)));

        var act = () => BinaryExpressionMapper.Instance.ToProto(remote, WriteContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("Binary operator 2147483647 is not suported");
    }

    [Theory]
    [MemberData(nameof(AllUnaryOperators))]
    public void Should_map_unary_operator_both_ways(RemoteLinq.UnaryOperator op, Proto.UnaryExpression.Types.UnaryOperator protoOp)
    {
        var remote = new RemoteLinq.UnaryExpression(op, new RemoteLinq.ConstantExpression(42, typeof(int)), typeof(int), null);

        var proto = UnaryExpressionMapper.Instance.ToProto(remote, WriteContext);
        proto.UnaryOperator.ShouldBe(protoOp);

        var clone = UnaryExpressionMapper.Instance.FromProto(new Proto.UnaryExpression { UnaryOperator = protoOp }, ReadContext);
        clone.UnaryOperator.ShouldBe(op);
    }

    [Fact]
    public void Should_map_null_unary_expression_to_null_proto()
        => UnaryExpressionMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_unary_proto_to_null_expression()
        => UnaryExpressionMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_unknown_unary_operator_in_proto()
    {
        var act = () => UnaryExpressionMapper.Instance.FromProto(new Proto.UnaryExpression { UnaryOperator = (Proto.UnaryExpression.Types.UnaryOperator)int.MaxValue }, ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("Unary operator 2147483647 is not suported");
    }

    [Fact]
    public void Should_throw_for_unknown_unary_operator_in_remote_expression()
    {
        var remote = new RemoteLinq.UnaryExpression((RemoteLinq.UnaryOperator)int.MaxValue, new RemoteLinq.ConstantExpression(42, typeof(int)), typeof(int), null);

        var act = () => UnaryExpressionMapper.Instance.ToProto(remote, WriteContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("Unary operator 2147483647 is not suported");
    }

    [Theory]
    [InlineData(RemoteLinq.GotoExpressionKind.Break, Proto.GotoExpression.Types.GotoExpressionKind.Break)]
    [InlineData(RemoteLinq.GotoExpressionKind.Continue, Proto.GotoExpression.Types.GotoExpressionKind.Continue)]
    [InlineData(RemoteLinq.GotoExpressionKind.Goto, Proto.GotoExpression.Types.GotoExpressionKind.Goto)]
    [InlineData(RemoteLinq.GotoExpressionKind.Return, Proto.GotoExpression.Types.GotoExpressionKind.Return)]
    public void Should_map_goto_expression_kind_both_ways(RemoteLinq.GotoExpressionKind kind, Proto.GotoExpression.Types.GotoExpressionKind protoKind)
    {
        var remote = new RemoteLinq.GotoExpression(kind, new RemoteLinq.LabelTarget("goto", typeof(int)), typeof(int), null);

        var proto = GotoExpressionMapper.Instance.ToProto(remote, WriteContext);
        proto.Kind.ShouldBe(protoKind);

        var clone = GotoExpressionMapper.Instance.FromProto(new Proto.GotoExpression { Kind = protoKind }, ReadContext);
        clone.Kind.ShouldBe(kind);
    }

    [Fact]
    public void Should_map_null_goto_expression_to_null_proto()
        => GotoExpressionMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_goto_proto_to_null_expression()
        => GotoExpressionMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_unknown_goto_expression_kind()
    {
        var act = () => GotoExpressionMapper.Instance.FromProto(new Proto.GotoExpression { Kind = (Proto.GotoExpression.Types.GotoExpressionKind)int.MaxValue }, ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("Goto expression kind 2147483647 is not suported");
    }

    [Theory]
    [InlineData(RemoteLinq.NewArrayType.NewArrayBounds, Proto.NewArrayExpression.Types.NewArrayType.NewArrayBounds)]
    [InlineData(RemoteLinq.NewArrayType.NewArrayInit, Proto.NewArrayExpression.Types.NewArrayType.NewArrayInit)]
    public void Should_map_new_array_type_both_ways(RemoteLinq.NewArrayType newArrayType, Proto.NewArrayExpression.Types.NewArrayType protoType)
    {
        var remote = new RemoteLinq.NewArrayExpression(newArrayType, typeof(int), [new RemoteLinq.ConstantExpression(1, typeof(int))]);

        var proto = NewArrayExpressionMapper.Instance.ToProto(remote, WriteContext);
        proto.NewArrayType.ShouldBe(protoType);

        var clone = NewArrayExpressionMapper.Instance.FromProto(new Proto.NewArrayExpression { NewArrayType = protoType }, ReadContext);
        clone.NewArrayType.ShouldBe(newArrayType);
    }

    [Fact]
    public void Should_map_null_new_array_expression_to_null_proto()
        => NewArrayExpressionMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_new_array_proto_to_null_expression()
        => NewArrayExpressionMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_unknown_new_array_type()
    {
        var act = () => NewArrayExpressionMapper.Instance.FromProto(new Proto.NewArrayExpression { NewArrayType = (Proto.NewArrayExpression.Types.NewArrayType)int.MaxValue }, ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("New array type 2147483647 is not suported");
    }

    [Theory]
    [MemberData(nameof(MemberBindingCases))]
    public void Should_map_member_binding_oneof_case_both_ways(RemoteLinq.MemberBinding value, Proto.MemberBinding.KindOneofCase expectedKindCase)
    {
        var proto = MemberBindingMapper.Instance.ToProto(value, WriteContext);
        proto.KindCase.ShouldBe(expectedKindCase);

        var clone = MemberBindingMapper.Instance.FromProto(proto, ReadContext);
        clone.ShouldBeOfType(value.GetType());
        clone.BindingType.ShouldBe(value.BindingType);
    }

    [Fact]
    public void Should_map_null_member_binding_to_null_proto()
        => MemberBindingMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_member_binding_proto_to_null_binding()
        => MemberBindingMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_proto_member_binding_without_selected_oneof_case()
    {
        var act = () => MemberBindingMapper.Instance.FromProto(new Proto.MemberBinding(), ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("None is not supported");
    }

    [Theory]
    [MemberData(nameof(ConstantValueCases))]
    public void Should_map_constant_value_oneof_case_both_ways(object value, Proto.ConstantValue.KindOneofCase expectedKindCase, Action<object> assert)
    {
        var proto = ConstantValueMapper.Instance.ToProto(value, WriteContext);
        proto.KindCase.ShouldBe(expectedKindCase);

        var clone = ConstantValueMapper.Instance.FromProto(proto, ReadContext);
        assert(clone);
    }

    [Fact]
    public void Should_map_null_constant_value_to_null_proto()
        => ConstantValueMapper.Instance.ToProto(null, WriteContext).ShouldBeNull();

    [Fact]
    public void Should_map_null_constant_value_proto_to_null_value()
        => ConstantValueMapper.Instance.FromProto(null, ReadContext).ShouldBeNull();

    [Fact]
    public void Should_throw_for_proto_constant_value_without_selected_oneof_case()
    {
        var act = () => ConstantValueMapper.Instance.FromProto(new Proto.ConstantValue(), ReadContext);

        var exception = act.ShouldThrow<ProtobufSerializationException>();
        exception.Message.ShouldBe("None is not supported");
    }

    public static IEnumerable<object[]> AllBinaryOperators
    {
        get
        {
            foreach (var op in (RemoteLinq.BinaryOperator[])Enum.GetValues(typeof(RemoteLinq.BinaryOperator)))
            {
                yield return [op, (Proto.BinaryExpression.Types.BinaryOperator)Enum.Parse(typeof(Proto.BinaryExpression.Types.BinaryOperator), op.ToString())];
            }
        }
    }

    public static IEnumerable<object[]> AllUnaryOperators
    {
        get
        {
            foreach (var op in (RemoteLinq.UnaryOperator[])Enum.GetValues(typeof(RemoteLinq.UnaryOperator)))
            {
                yield return [op, (Proto.UnaryExpression.Types.UnaryOperator)Enum.Parse(typeof(Proto.UnaryExpression.Types.UnaryOperator), op.ToString())];
            }
        }
    }

    public static TheoryData<RemoteLinq.MemberBinding, Proto.MemberBinding.KindOneofCase> MemberBindingCases
    {
        get
        {
            var data = new TheoryData<RemoteLinq.MemberBinding, Proto.MemberBinding.KindOneofCase>();
            data.Add(
                new RemoteLinq.MemberAssignment(UnionTagExpressionFactory.IntMaxValueMember, new RemoteLinq.ConstantExpression(0, typeof(int))),
                Proto.MemberBinding.KindOneofCase.MemberAssignment);

            var elementInit = new RemoteLinq.ElementInit(typeof(List<int>).GetMethod(nameof(List<int>.Add)), new List<RemoteLinq.Expression> { new RemoteLinq.ConstantExpression(1, typeof(int)) });
            data.Add(
                new RemoteLinq.MemberListBinding(UnionTagExpressionFactory.IntMaxValueMember, new List<RemoteLinq.ElementInit> { elementInit }),
                Proto.MemberBinding.KindOneofCase.MemberListBinding);

            var nestedAssignment = new RemoteLinq.MemberAssignment(UnionTagExpressionFactory.IntMaxValueMember, new RemoteLinq.ConstantExpression(1, typeof(int)));
            data.Add(
                new RemoteLinq.MemberMemberBinding(UnionTagExpressionFactory.IntMaxValueMember, new List<RemoteLinq.MemberBinding> { nestedAssignment }),
                Proto.MemberBinding.KindOneofCase.MemberMemberBinding);
            return data;
        }
    }

    public static TheoryData<object, Proto.ConstantValue.KindOneofCase, Action<object>> ConstantValueCases
    {
        get
        {
            var data = new TheoryData<object, Proto.ConstantValue.KindOneofCase, Action<object>>();
            data.Add(42, Proto.ConstantValue.KindOneofCase.Value, value => value.ShouldBe(42));
            data.Add(
                new ConstantQueryArgument(new DynamicObject(new[] { ("Count", (object)42) })),
                Proto.ConstantValue.KindOneofCase.ConstantQueryArgument,
                value => ((ConstantQueryArgument)value).Value.Properties["Count"].ShouldBe(42));
            data.Add(
                new VQArg(42, typeof(int)),
                Proto.ConstantValue.KindOneofCase.VariableQueryArgument,
                value =>
                {
                    var argument = (VQArg)value;
                    argument.Value.ShouldBe(42);
                    argument.Type.ToType().ShouldBe(typeof(int));
                });
            data.Add(
                new VariableQueryArgumentList(new List<int> { 1, 2 }, typeof(int)),
                Proto.ConstantValue.KindOneofCase.VariableQueryArgumentList,
                value =>
                {
                    var list = (VariableQueryArgumentList)value;
                    list.ElementType.ToType().ShouldBe(typeof(int));
                    list.Values.ShouldContain(1);
                    list.Values.ShouldContain(2);
                });
            data.Add(new SubstitutionValue(typeof(int)), Proto.ConstantValue.KindOneofCase.SubstitutionValue, value => ((SubstitutionValue)value).Type.ToType().ShouldBe(typeof(int)));
            data.Add(new QueryableResourceDescriptor(typeof(int)), Proto.ConstantValue.KindOneofCase.QueryableResourceDescriptor, value => ((QueryableResourceDescriptor)value).Type.ToType().ShouldBe(typeof(int)));
            return data;
        }
    }
}
