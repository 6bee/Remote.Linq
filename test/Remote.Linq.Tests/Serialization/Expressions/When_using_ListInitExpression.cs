// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization.Expressions;

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Xunit;
using RemoteLambdaExpression = Remote.Linq.Expressions.LambdaExpression;

public abstract class When_using_ListInitExpression
{
    public class With_data_contract_serializer() : When_using_ListInitExpression(DataContractSerializationHelper.CloneExpression);

    public class With_newtonsoft_json_serializer() : When_using_ListInitExpression(NewtonsoftJsonSerializationHelper.Clone);

    public class With_system_text_json_serializer() : When_using_ListInitExpression(SystemTextJsonSerializationHelper.Clone);

    public class With_protobuf_serializer() : When_using_ListInitExpression(ProtobufSerializationHelper.Clone);

    public class With_messagepack_serializer() : When_using_ListInitExpression(MessagePackSerializationHelper.Clone);

    public class With_xml_serializer() : When_using_ListInitExpression(XmlSerializationHelper.CloneExpression);

#if NETFRAMEWORK
    public class With_binary_formatter() : When_using_ListInitExpression(BinarySerializationHelper.Clone);

    public class With_net_data_contract_serializer() : When_using_ListInitExpression(NetDataContractSerializationHelper.Clone);
#endif // NETFRAMEWORK

    private readonly Expression<Func<List<int>>> _originalExpression;

    private readonly RemoteLambdaExpression _remoteExpression;

    private readonly RemoteLambdaExpression _serializedRemoteExpression;

    [SuppressMessage("Minor Code Smell", "S3220:Method calls should not resolve ambiguously to overloads with \"params\"", Justification = "Intentional test setup")]
    protected When_using_ListInitExpression(Func<RemoteLambdaExpression, RemoteLambdaExpression> serialize)
    {
        var expression = Expression.Lambda<Func<List<int>>>(
            Expression.ListInit(
                Expression.New(typeof(List<int>).GetConstructors().First()),
                new[]
                {
                    Expression.ElementInit(
                        typeof(List<int>).GetMethod(nameof(List<int>.Add))!,
                        new[] { Expression.Constant(1) }),
                    Expression.ElementInit(
                        typeof(List<int>).GetMethod(nameof(List<int>.Add))!,
                        new[] { Expression.Constant(2) }),
                    Expression.ElementInit(
                        typeof(List<int>).GetMethod(nameof(List<int>.Add))!,
                        new[] { Expression.Constant(3) }),
                }));

        _originalExpression = expression;

        _remoteExpression = expression.ToRemoteLinqExpression();

        _serializedRemoteExpression = serialize(_remoteExpression);
    }

    [Fact]
    public void Expression_result_should_contain_correct_values()
    {
        var list1 = _originalExpression.Compile()();
        var list2 = _remoteExpression.ToLinqExpression<Func<List<int>>>().Compile()();
        var list3 = _serializedRemoteExpression.ToLinqExpression<Func<List<int>>>().Compile()();

        list1.ShouldNotBeNull();
        list2.ShouldNotBeNull();
        list3.ShouldNotBeNull();

        list1.Count.ShouldBe(3);
        list2.Count.ShouldBe(3);
        list3.Count.ShouldBe(3);

        list1[0].ShouldBe(1);
        list2[0].ShouldBe(1);
        list3[0].ShouldBe(1);

        list1[1].ShouldBe(2);
        list2[1].ShouldBe(2);
        list3[1].ShouldBe(2);

        list1[2].ShouldBe(3);
        list2[2].ShouldBe(3);
        list3[2].ShouldBe(3);
    }
}
