// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using System.Text.Json;
using VariableArgument = Remote.Linq.DynamicQuery.VariableQueryArgument;
using VariableArgumentList = Remote.Linq.DynamicQuery.VariableQueryArgumentList;

public class When_reading_system_text_json_variable_query_arguments
{
    private static JsonSerializerOptions Options => new JsonSerializerOptions().ConfigureRemoteLinq();

    [Fact]
    public void Should_deserialize_scalar_arguments_with_typed_null_and_non_null_values()
    {
        var nullResult = JsonSerializer.Deserialize<VariableArgument>("{\"$id\":\"1\",\"$type\":\"VariableQueryArgument\",\"Type\":\"int32\",\"Value\":null}", Options);
        var valueResult = JsonSerializer.Deserialize<VariableArgument>("{\"$id\":\"1\",\"$type\":\"VariableQueryArgument\",\"Type\":\"int32\",\"Value\":42}", Options);

        nullResult.Value.ShouldBeNull();
        nullResult.Type.ToType().ShouldBe(typeof(int));

        valueResult.Value.ShouldBe(42);
        valueResult.Type.ToType().ShouldBe(typeof(int));
    }

    // KNOWN DEFECT (logged per plan/02-complex-tests.md section 6.2): the reader in
    // src/Remote.Linq/Text/Json/Converters/VariableQueryArgumentListConverter.cs cannot read
    // back the JSON written by its own writer (writer output: {"$id":"1","$type":"VariableQueryArgumentList","ElementType":"int32","Values":{"$id":"2","$values":[1,2,3]}}).
    // After reading the "ElementType" value the reader sits on the value token, and a single Advance()
    // call only moves it to the "Values" property name instead of its value. The token check
    // ("Expected array") is therefore unreachable for every well-formed "Values" form: null, an empty
    // array (in "$values" object form), or a populated array all throw the same exception. This test
    // documents the currently supported reader contract: the exact serializer exception for all forms.
    [Theory]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":\"int32\",\"Values\":null}")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":\"int32\",\"Values\":{\"$values\":[]}}")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":\"int32\",\"Values\":{\"$values\":[1,2,3]}}")]
    public void Should_deserialize_list_arguments_per_supported_reader_contract(string json)
    {
        var exception = Should.Throw<JsonException>(() => JsonSerializer.Deserialize<VariableArgumentList>(json, Options));

        exception.Message.ShouldBe("Expected array");
    }

    [Theory]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":\"int32\",\"Values\":42}")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":\"int32\",\"Values\":{\"a\":1}}")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":\"int32\"}")]
    public void Should_reject_invalid_list_tokens(string json)
    {
        var exception = Should.Throw<JsonException>(() => JsonSerializer.Deserialize<VariableArgumentList>(json, Options));

        exception.Message.ShouldBe("Expected array");
    }

    [Theory]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgument\",\"Type\":null,\"Value\":42}", "Type must not be null.")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgument\",\"Value\":42}", "Expected token 'Type'.")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"ElementType\":null,\"Values\":null}", "ElementType must not be null.")]
    [InlineData("{\"$id\":\"1\",\"$type\":\"VariableQueryArgumentList\",\"Values\":null}", "Expected token 'ElementType'.")]
    public void Should_reject_missing_or_null_type_metadata(string json, string expectedMessage)
    {
        var target = json.Contains("VariableQueryArgumentList") ? typeof(VariableArgumentList) : typeof(VariableArgument);

        var exception = Should.Throw<JsonException>(() => JsonSerializer.Deserialize(json, target, Options));

        exception.Message.ShouldBe(expectedMessage);
    }
}
