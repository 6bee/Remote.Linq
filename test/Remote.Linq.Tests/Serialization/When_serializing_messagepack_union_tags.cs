// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua.Dynamic;
using Aqua.MessagePack;
using Aqua.MessagePack.Formatters;
using Aqua.TypeSystem;
using global::MessagePack;
using global::MessagePack.Formatters;
using Remote.Linq.DynamicQuery;
using Remote.Linq.MessagePack;
using Remote.Linq.MessagePack.Formatters;
using System.Buffers;
using System.IO;
using RemoteLinq = Remote.Linq.Expressions;
using VQArg = Remote.Linq.DynamicQuery.VariableQueryArgument;

/// <summary>
/// Verifies the MessagePack tagged-union wire format for remote expressions, constant values,
/// member bindings and the <see cref="RemoteLinqFormatterResolver"/>.
/// </summary>
public class When_serializing_messagepack_union_tags
{
    private static readonly AquaMessagePackSerializerOptions Options = MessagePackSerializerOptions.Standard.ConfigureRemoteLinq();

    private static readonly MemberInfo PersonTagsMember = MemberInfo.Create(typeof(Person).GetProperty(nameof(Person.Tags)));

    private static readonly MemberInfo PersonAddressMember = MemberInfo.Create(typeof(Person).GetProperty(nameof(Person.Address)));

    private static readonly MemberInfo AddressCityMember = MemberInfo.Create(typeof(Address).GetProperty(nameof(Address.City)));

    [Fact]
    public void Should_roundtrip_null_expression()
    {
        RemoteLinq.Expression expression = null;

        MessagePackSerializationHelper.Clone(expression).ShouldBeNull();
    }

    [Theory]
    [MemberData(nameof(AllExpressionTags))]
    public void Should_roundtrip_expression_for_every_node_type_tag(RemoteLinq.ExpressionType nodeType)
    {
        var expression = UnionTagExpressionFactory.Build(nodeType);

        var clone = MessagePackSerializationHelper.Clone(expression);

        clone.NodeType.ShouldBe(nodeType);
        clone.ShouldBeOfType(expression.GetType());
    }

    [Fact]
    public void Should_throw_for_empty_expression_array_payload()
    {
        var act = new Action(() =>
        {
            var reader = new MessagePackReader(new byte[] { 0x90 });
            ExpressionFormatter.Instance.Deserialize(ref reader, Options);
        });

        var exception = act.ShouldThrow<MessagePackSerializationException>();
        exception.Message.ShouldBe("Empty expression array.");
    }

    [Fact]
    public void Should_throw_for_unknown_expression_type_tag()
    {
        var act = new Action(() =>
        {
            // fixarray(1) followed by int32(999) = 0xD2 0x00 0x00 0x03 0xE7
            var reader = new MessagePackReader(new byte[] { 0x91, 0xD2, 0x00, 0x00, 0x03, 0xE7 });
            ExpressionFormatter.Instance.Deserialize(ref reader, Options);
        });

        var exception = act.ShouldThrow<MessagePackSerializationException>();
        exception.Message.ShouldBe("Unknown expression type tag: 999");
    }

    [Theory]
    [MemberData(nameof(ConstantValueTags))]
    public void Should_roundtrip_constant_value_for_every_tag(object value, Action<object> assert)
    {
        var clone = RoundtripConstantValue(value);

        assert(clone);
    }

    [Fact]
    public void Should_roundtrip_null_constant_value()
    {
        object value = null;

        RoundtripConstantValue(value).ShouldBeNull();
    }

    [Fact]
    public void Should_return_null_for_empty_constant_value_array_payload()
    {
        var reader = new MessagePackReader(new byte[] { 0x90 });

        ConstantValueFormatter.Instance.Deserialize(ref reader, Options).ShouldBeNull();
    }

    [Fact]
    public void Should_skip_trailing_items_in_constant_value_payload()
    {
        var stream = new MemoryStream();
        var bw = new MemoryStreamBufferWriter(stream);
        var writer = new MessagePackWriter(bw);
        writer.WriteArrayHeader(3);
        writer.Write((byte)0);
        AquaValueFormatter.Instance.Serialize(ref writer, 42, Options);
        writer.Write(99);
        writer.Flush();

        var reader = new MessagePackReader(stream.ToArray());
        ConstantValueFormatter.Instance.Deserialize(ref reader, Options).ShouldBe(42);
    }

    [Fact]
    public void Should_throw_for_unknown_constant_value_tag()
    {
        var act = new Action(() =>
        {
            // fixarray(2), uint8(255) = 0xCC 0xFF, int 0 (fixint 0x00)
            var reader = new MessagePackReader(new byte[] { 0x92, 0xCC, 0xFF, 0x00 });
            ConstantValueFormatter.Instance.Deserialize(ref reader, Options);
        });

        var exception = act.ShouldThrow<MessagePackSerializationException>();
        exception.Message.ShouldBe("Unknown constant value tag: 255");
    }

    [Theory]
    [MemberData(nameof(MemberBindingTags))]
    public void Should_roundtrip_member_binding_for_every_tag(RemoteLinq.MemberBinding value, Action<RemoteLinq.MemberBinding> assert)
    {
        var clone = RoundtripMemberBinding(value);

        assert(clone);
    }

    [Fact]
    public void Should_roundtrip_null_member_binding()
    {
        RemoteLinq.MemberBinding value = null;

        RoundtripMemberBinding(value).ShouldBeNull();
    }

    [Fact]
    public void Should_throw_for_empty_member_binding_array_payload()
    {
        var act = new Action(() =>
        {
            var reader = new MessagePackReader(new byte[] { 0x90 });
            MemberBindingFormatter.Instance.Deserialize(ref reader, Options);
        });

        var exception = act.ShouldThrow<MessagePackSerializationException>();
        exception.Message.ShouldBe("Empty member binding array.");
    }

    [Fact]
    public void Should_throw_for_unknown_member_binding_type_tag()
    {
        var act = new Action(() =>
        {
            // fixarray(1) followed by int32(999) = 0xD2 0x00 0x00 0x03 0xE7
            var reader = new MessagePackReader(new byte[] { 0x91, 0xD2, 0x00, 0x00, 0x03, 0xE7 });
            MemberBindingFormatter.Instance.Deserialize(ref reader, Options);
        });

        var exception = act.ShouldThrow<MessagePackSerializationException>();
        exception.Message.ShouldBe("Unknown MemberBindingType tag: 999");
    }

    [Theory]
    [MemberData(nameof(RegisteredFormatters))]
    public void Should_resolve_registered_formatter_for_every_remote_linq_type(Type type, object expectedFormatter)
    {
        var resolver = new RemoteLinqFormatterResolver();

        GetFormatter(resolver, type).ShouldBeSameAs(expectedFormatter);
    }

    [Fact]
    public void Should_fall_back_to_fallback_resolver_for_unregistered_type()
    {
        var resolver = new RemoteLinqFormatterResolver(SentinelResolver.Instance);

        GetFormatter(resolver, typeof(string)).ShouldBeOfType<SentinelFormatter<string>>();
    }

    public static IEnumerable<object[]> AllExpressionTags =>
        ((RemoteLinq.ExpressionType[])Enum.GetValues(typeof(RemoteLinq.ExpressionType))).Select(x => new object[] { x });

    public static TheoryData<object, Action<object>> ConstantValueTags
    {
        get
        {
            var data = new TheoryData<object, Action<object>>();
            data.Add(42, value => value.ShouldBe(42));
            data.Add(
                new ConstantQueryArgument(new DynamicObject(new[] { ("Count", (object)42) })),
                value => ((ConstantQueryArgument)value).Value.Properties["Count"].ShouldBe(42));
            data.Add(
                new VQArg(42, typeof(int)),
                value =>
                {
                    var argument = (VQArg)value;
                    argument.Value.ShouldBe(42);
                    argument.Type.ToType().ShouldBe(typeof(int));
                });
            data.Add(
                new VariableQueryArgumentList(new List<int> { 1, 2 }, typeof(int)),
                value =>
                {
                    var list = (VariableQueryArgumentList)value;
                    list.ElementType.ToType().ShouldBe(typeof(int));
                    list.Values.ShouldContain(1);
                    list.Values.ShouldContain(2);
                });
            data.Add(new SubstitutionValue(typeof(int)), value => ((SubstitutionValue)value).Type.ToType().ShouldBe(typeof(int)));
            data.Add(new QueryableResourceDescriptor(typeof(int)), value => ((QueryableResourceDescriptor)value).Type.ToType().ShouldBe(typeof(int)));
            return data;
        }
    }

    public static TheoryData<RemoteLinq.MemberBinding, Action<RemoteLinq.MemberBinding>> MemberBindingTags
    {
        get
        {
            var data = new TheoryData<RemoteLinq.MemberBinding, Action<RemoteLinq.MemberBinding>>();
            data.Add(
                new RemoteLinq.MemberAssignment(UnionTagExpressionFactory.IntMaxValueMember, new RemoteLinq.ConstantExpression(0, typeof(int))),
                binding => ((RemoteLinq.MemberAssignment)binding).BindingType.ShouldBe(RemoteLinq.MemberBindingType.Assignment));

            var elementInit = new RemoteLinq.ElementInit(typeof(List<int>).GetMethod(nameof(List<int>.Add)), new List<RemoteLinq.Expression> { new RemoteLinq.ConstantExpression(1, typeof(int)) });
            data.Add(
                new RemoteLinq.MemberListBinding(PersonTagsMember, new List<RemoteLinq.ElementInit> { elementInit }),
                binding => ((RemoteLinq.MemberListBinding)binding).BindingType.ShouldBe(RemoteLinq.MemberBindingType.ListBinding));

            var cityAssignment = new RemoteLinq.MemberAssignment(AddressCityMember, new RemoteLinq.ConstantExpression("x", typeof(string)));
            data.Add(
                new RemoteLinq.MemberMemberBinding(PersonAddressMember, new List<RemoteLinq.MemberBinding> { cityAssignment }),
                binding => ((RemoteLinq.MemberMemberBinding)binding).BindingType.ShouldBe(RemoteLinq.MemberBindingType.MemberBinding));
            return data;
        }
    }

    public static IEnumerable<object[]> RegisteredFormatters =>
    [
         [typeof(RemoteLinq.Expression), ExpressionFormatter.Instance],
         [typeof(RemoteLinq.BinaryExpression), BinaryExpressionFormatter.Instance],
         [typeof(RemoteLinq.BlockExpression), BlockExpressionFormatter.Instance],
         [typeof(RemoteLinq.ConditionalExpression), ConditionalExpressionFormatter.Instance],
         [typeof(RemoteLinq.ConstantExpression), ConstantExpressionFormatter.Instance],
         [typeof(RemoteLinq.DefaultExpression), DefaultExpressionFormatter.Instance],
         [typeof(RemoteLinq.GotoExpression), GotoExpressionFormatter.Instance],
         [typeof(RemoteLinq.InvokeExpression), InvokeExpressionFormatter.Instance],
         [typeof(RemoteLinq.LabelExpression), LabelExpressionFormatter.Instance],
         [typeof(RemoteLinq.LambdaExpression), LambdaExpressionFormatter.Instance],
         [typeof(RemoteLinq.ListInitExpression), ListInitExpressionFormatter.Instance],
         [typeof(RemoteLinq.LoopExpression), LoopExpressionFormatter.Instance],
         [typeof(RemoteLinq.MemberExpression), MemberExpressionFormatter.Instance],
         [typeof(RemoteLinq.MemberInitExpression), MemberInitExpressionFormatter.Instance],
         [typeof(RemoteLinq.MethodCallExpression), MethodCallExpressionFormatter.Instance],
         [typeof(RemoteLinq.NewExpression), NewExpressionFormatter.Instance],
         [typeof(RemoteLinq.NewArrayExpression), NewArrayExpressionFormatter.Instance],
         [typeof(RemoteLinq.ParameterExpression), ParameterExpressionFormatter.Instance],
         [typeof(RemoteLinq.SwitchExpression), SwitchExpressionFormatter.Instance],
         [typeof(RemoteLinq.TryExpression), TryExpressionFormatter.Instance],
         [typeof(RemoteLinq.TypeBinaryExpression), TypeBinaryExpressionFormatter.Instance],
         [typeof(RemoteLinq.UnaryExpression), UnaryExpressionFormatter.Instance],
         [typeof(RemoteLinq.CatchBlock), CatchBlockFormatter.Instance],
         [typeof(RemoteLinq.LabelTarget), LabelTargetFormatter.Instance],
         [typeof(RemoteLinq.SwitchCase), SwitchCaseFormatter.Instance],
         [typeof(RemoteLinq.ElementInit), ElementInitFormatter.Instance],
         [typeof(RemoteLinq.MemberBinding), MemberBindingFormatter.Instance],
         [typeof(RemoteLinq.MemberAssignment), MemberAssignmentFormatter.Instance],
         [typeof(RemoteLinq.MemberListBinding), MemberListBindingFormatter.Instance],
         [typeof(RemoteLinq.MemberMemberBinding), MemberMemberBindingFormatter.Instance],
         [typeof(ConstantQueryArgument), ConstantQueryArgumentFormatter.Instance],
         [typeof(VQArg), VariableQueryArgumentFormatter.Instance],
         [typeof(VariableQueryArgumentList), VariableQueryArgumentListFormatter.Instance],
         [typeof(SubstitutionValue), SubstitutionValueFormatter.Instance],
         [typeof(QueryableResourceDescriptor), QueryableResourceDescriptorFormatter.Instance],
    ];

    private static object RoundtripConstantValue(object value)
    {
        var stream = new MemoryStream();
        var bw = new MemoryStreamBufferWriter(stream);
        var writer = new MessagePackWriter(bw);
        ConstantValueFormatter.Instance.Serialize(ref writer, value, Options);
        writer.Flush();

        var reader = new MessagePackReader(stream.ToArray());
        return ConstantValueFormatter.Instance.Deserialize(ref reader, Options);
    }

    private static RemoteLinq.MemberBinding RoundtripMemberBinding(RemoteLinq.MemberBinding value)
    {
        var stream = new MemoryStream();
        var bw = new MemoryStreamBufferWriter(stream);
        var writer = new MessagePackWriter(bw);
        MemberBindingFormatter.Instance.Serialize(ref writer, value, Options);
        writer.Flush();

        var reader = new MessagePackReader(stream.ToArray());
        return MemberBindingFormatter.Instance.Deserialize(ref reader, Options);
    }

    private static object GetFormatter(IFormatterResolver resolver, Type type)
        => typeof(IFormatterResolver).GetMethod(nameof(IFormatterResolver.GetFormatter)).MakeGenericMethod(type).Invoke(resolver, null);

    private sealed class Person
    {
        public List<int> Tags { get; set; } = [];

        public Address Address { get; set; } = new();
    }

    private sealed class Address
    {
        public string City { get; set; } = string.Empty;
    }

    private sealed class MemoryStreamBufferWriter(MemoryStream stream) : IBufferWriter<byte>
    {
        private byte[] _buffer = new byte[4096];

        private int _offset;

        public void Advance(int count)
        {
            stream.Write(_buffer, _offset, count);
            _offset += count;
        }

        public Memory<byte> GetMemory(int sizeHint = 0)
            => new(_buffer, _offset, _buffer.Length - _offset);

        public Span<byte> GetSpan(int sizeHint = 0)
            => new(_buffer, _offset, _buffer.Length - _offset);

        public void Complete() => _offset = 0;
    }

    private sealed class SentinelResolver : IFormatterResolver
    {
        public static readonly SentinelResolver Instance = new();

        public static SentinelFormatter<T> CreateFormatter<T>()
            => new();

        public IMessagePackFormatter<T> GetFormatter<T>()
            => typeof(T) == typeof(string) ? CreateFormatter<T>() : null;
    }

    internal sealed class SentinelFormatter<T> : IMessagePackFormatter<T>
    {
        public void Serialize(ref MessagePackWriter writer, T value, MessagePackSerializerOptions options)
            => writer.Write((string)(object)value!);

        public T Deserialize(ref MessagePackReader reader, MessagePackSerializerOptions options)
            => (T)(object)reader.ReadString();
    }
}
