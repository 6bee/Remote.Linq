// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_MemberExpression
{
    public class With_data_contract_serializer() : When_using_MemberExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_MemberExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_MemberExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_using_MemberExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_using_MemberExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_MemberExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_using_MemberExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_using_MemberExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<Person, string>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_using_MemberExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        var p = Expression.Parameter(typeof(Person), "person");
        var expression = Expression.Lambda<Func<Person, string>>(
            Expression.Property(p, nameof(Person.Name)),
            p);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_return_name()
    {
        var person = new Person { Name = "Alice" };

        string result1 = _originalExpression.Compile()(person);

        string result2 = _remoteExpression.ToLinqExpression<Func<Person, string>>().Compile()(person);

        string result3 = _serializedRemoteExpression.ToLinqExpression<Func<Person, string>>().Compile()(person);

        "Alice"
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    [Fact]
    public void Expression_result_should_return_empty_name()
    {
        var person = new Person { Name = "Test" };

        string result1 = _originalExpression.Compile()(person);

        string result2 = _remoteExpression.ToLinqExpression<Func<Person, string>>().Compile()(person);

        string result3 = _serializedRemoteExpression.ToLinqExpression<Func<Person, string>>().Compile()(person);

        "Test"
            .ShouldMatch(result1)
            .ShouldMatch(result2)
            .ShouldMatch(result3);
    }

    private sealed class Person
    {
        public string Name { get; set; } = null!;
    }
}
