// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Remote.Linq.Protobuf;
using System.Buffers;
using System.IO;
using RemoteLinq = Remote.Linq.Expressions;
using SystemLinq = System.Linq.Expressions;

/// <summary>
/// Verifies every <see cref="RemoteLinqProtobufSerializer"/> serialization and
/// deserialization overload by round-tripping remote Linq expression graphs through
/// the supported write targets (byte array, stream, span and buffer writer).
/// </summary>
public abstract class When_serializing_remote_linq_graphs_with_protobuf
{
    private readonly SystemLinq.Expression<Func<int, int, bool>> _originalExpression;
    private readonly RemoteLinq.LambdaExpression _remoteExpression;

    protected When_serializing_remote_linq_graphs_with_protobuf()
    {
        _originalExpression = BuildExpression(SystemLinq.Expression.GreaterThan);
        _remoteExpression = _originalExpression.ToRemoteLinqExpression();
    }

    protected abstract RemoteLinq.LambdaExpression Roundtrip(RemoteLinq.LambdaExpression graph);

    [Fact]
    public void Should_roundtrip_and_evaluate_equivalently_to_original_expression()
    {
        var original = _originalExpression.Compile();
        var roundtripped = Roundtrip(_remoteExpression).ToLinqExpression<Func<int, int, bool>>().Compile();

        roundtripped(5, 3).ShouldBe(original(5, 3));
        roundtripped(3, 5).ShouldBe(original(3, 5));
        roundtripped(7, 7).ShouldBe(original(7, 7));
    }

    [Fact]
    public void Should_preserve_lambda_expression_metadata()
    {
        var roundtripped = Roundtrip(_remoteExpression);

        roundtripped.ShouldNotBeNull();
        roundtripped.NodeType.ShouldBe(RemoteLinq.ExpressionType.Lambda);
        ((Type)roundtripped.Type).ShouldBe(typeof(Func<int, int, bool>));
        roundtripped.Parameters.Count.ShouldBe(2);
    }

    [Theory]
    [MemberData(nameof(DistinctExpressions))]
    public void Should_roundtrip_distinct_expression_graphs_and_preserve_behavior(SystemLinq.Expression<Func<int, int, bool>> expression, int left, int right, bool expected)
    {
        var roundtripped = Roundtrip(expression.ToRemoteLinqExpression()).ToLinqExpression<Func<int, int, bool>>().Compile();

        roundtripped(left, right).ShouldBe(expected);
    }

    public static IEnumerable<object[]> DistinctExpressions =>
    [
        [BuildExpression(SystemLinq.Expression.GreaterThan), 5, 3, true],
        [BuildExpression(SystemLinq.Expression.GreaterThan), 3, 5, false],
        [BuildExpression(SystemLinq.Expression.GreaterThanOrEqual), 3, 3, true],
        [BuildExpression(SystemLinq.Expression.GreaterThanOrEqual), 3, 4, false],
        [BuildExpression(SystemLinq.Expression.LessThan), 3, 5, true],
        [BuildExpression(SystemLinq.Expression.Equal), 7, 7, true],
        [BuildExpression(SystemLinq.Expression.Equal), 7, 8, false],
        [BuildExpression((a, b) => SystemLinq.Expression.GreaterThan(SystemLinq.Expression.Add(a, b), SystemLinq.Expression.Constant(5))), 4, 4, true],
    ];

    private static SystemLinq.Expression<Func<int, int, bool>> BuildExpression(Func<SystemLinq.ParameterExpression, SystemLinq.ParameterExpression, SystemLinq.Expression> body)
    {
        var a = SystemLinq.Expression.Parameter(typeof(int), "a");
        var b = SystemLinq.Expression.Parameter(typeof(int), "b");
        return SystemLinq.Expression.Lambda<Func<int, int, bool>>(body(a, b), a, b);
    }

    /// <summary>
    /// Round-trips the graph through the byte array serialize/deserialize overloads.
    /// </summary>
    public class Via_byte_array : When_serializing_remote_linq_graphs_with_protobuf
    {
        protected override RemoteLinq.LambdaExpression Roundtrip(RemoteLinq.LambdaExpression graph)
        {
            var data = RemoteLinqProtobufSerializer.Serialize(graph);
            return RemoteLinqProtobufSerializer.Deserialize<RemoteLinq.LambdaExpression>(data)!;
        }

        [Fact]
        public void Should_throw_argument_null_exception_when_deserializing_null_data()
        {
            byte[] data = null;
            var act = () => RemoteLinqProtobufSerializer.Deserialize<RemoteLinq.LambdaExpression>(data);

            var exception = act.ShouldThrow<ArgumentNullException>();
            exception.ParamName.ShouldBe("data");
        }
    }

    /// <summary>
    /// Round-trips the graph through the stream serialize/deserialize overloads.
    /// </summary>
    public class Via_memory_stream : When_serializing_remote_linq_graphs_with_protobuf
    {
        protected override RemoteLinq.LambdaExpression Roundtrip(RemoteLinq.LambdaExpression graph)
        {
            using var stream = new MemoryStream();
            RemoteLinqProtobufSerializer.Serialize(graph, stream);
            stream.Position = 0;
            return RemoteLinqProtobufSerializer.Deserialize<RemoteLinq.LambdaExpression>(stream)!;
        }

        [Fact]
        public void Should_throw_argument_null_exception_when_deserializing_null_stream()
        {
            Stream stream = null;
            var act = () => RemoteLinqProtobufSerializer.Deserialize<RemoteLinq.LambdaExpression>(stream);

            var exception = act.ShouldThrow<ArgumentNullException>();
            exception.ParamName.ShouldBe("stream");
        }
    }

    /// <summary>
    /// Round-trips the graph through the span serialize/deserialize overloads.
    /// </summary>
    public class Via_memory_span : When_serializing_remote_linq_graphs_with_protobuf
    {
        protected override RemoteLinq.LambdaExpression Roundtrip(RemoteLinq.LambdaExpression graph)
        {
            // The span overload requires a buffer of exactly the serialized size,
            // so the canonical byte array length determines the required capacity.
            var size = RemoteLinqProtobufSerializer.Serialize(graph).Length;
            var buffer = new byte[size];
            RemoteLinqProtobufSerializer.Serialize(graph, buffer);
            return RemoteLinqProtobufSerializer.Deserialize<RemoteLinq.LambdaExpression>(buffer)!;
        }

        [Fact]
        public void Should_throw_io_exception_when_target_span_is_too_small()
        {
            var remote = BuildExpression(SystemLinq.Expression.GreaterThan).ToRemoteLinqExpression();
            var buffer = new byte[1];
            var act = () => RemoteLinqProtobufSerializer.Serialize(remote, buffer.AsSpan());

            act.ShouldThrow<IOException>();
        }
    }

    /// <summary>
    /// Round-trips the graph through the buffer writer serialize/deserialize overloads.
    /// </summary>
    public class Via_buffer_writer : When_serializing_remote_linq_graphs_with_protobuf
    {
        protected override RemoteLinq.LambdaExpression Roundtrip(RemoteLinq.LambdaExpression graph)
        {
            var writer = new ArrayBufferWriter<byte>();
            RemoteLinqProtobufSerializer.Serialize(graph, writer);
            return RemoteLinqProtobufSerializer.Deserialize<RemoteLinq.LambdaExpression>(new ReadOnlySequence<byte>(writer.WrittenMemory))!;
        }
    }
}
