// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_serializing_MemberInitExpression
{
    public class With_data_contract_serializer() : When_serializing_MemberInitExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_serializing_MemberInitExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_serializing_MemberInitExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_serializing_MemberInitExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_serializing_MemberInitExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_serializing_MemberInitExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_serializing_MemberInitExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_serializing_MemberInitExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<int, Person>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_serializing_MemberInitExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        var age = Expression.Parameter(typeof(int), "age");
        var expression = Expression.Lambda<Func<int, Person>>(
            Expression.MemberInit(
                Expression.New(typeof(Person)),
                Expression.Bind(
                    typeof(Person).GetProperty(nameof(Person.Name))!,
                    Expression.Constant("John")),
                Expression.Bind(
                    typeof(Person).GetProperty(nameof(Person.Age))!,
                    age)),
            age);

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_have_correct_properties()
    {
        var argument = 30;

        var person1 = _originalExpression.Compile()(argument);
        var person2 = _remoteExpression.ToLinqExpression<Func<int, Person>>().Compile()(argument);
        var person3 = _serializedRemoteExpression.ToLinqExpression<Func<int, Person>>().Compile()(argument);

        "John"
            .ShouldMatch(person1.Name)
            .ShouldMatch(person2.Name)
            .ShouldMatch(person3.Name);

        argument
            .ShouldMatch(person1.Age)
            .ShouldMatch(person2.Age)
            .ShouldMatch(person3.Age);
    }

    private sealed class Person
    {
        public string Name { get; set; } = string.Empty;

        public int Age { get; set; }
    }
}
