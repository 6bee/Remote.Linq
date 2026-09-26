// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua;
using Remote.Linq.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;
using VariableArgument = Remote.Linq.DynamicQuery.VariableQueryArgument;
using VariableArgumentList = Remote.Linq.DynamicQuery.VariableQueryArgumentList;

public class When_serializing_system_text_json_expression_metadata
{
    private sealed class StubVariableQueryArgumentConverter : JsonConverter<VariableArgument>
    {
        public override VariableArgument Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            => throw new NotSupportedException();

        public override void Write(Utf8JsonWriter writer, VariableArgument value, JsonSerializerOptions options)
            => throw new NotSupportedException();
    }

    private static JsonSerializerOptions Options => new JsonSerializerOptions().ConfigureRemoteLinq();

    [Fact]
    public void Should_serialize_constant_with_value_type_differing_from_declared_type()
    {
        var constant = new ConstantExpression(42, typeof(object));

        var json = JsonSerializer.Serialize(constant, Options);

        json.ShouldContain("\"ValueType\":\"int32\"");
        json.ShouldContain("\"Name\":\"Object\"");
        json.ShouldContain("\"Value\":42");

        var result = JsonSerializer.Deserialize<ConstantExpression>(json, Options);

        result.Type.ToType().ShouldBe(typeof(object));
        result.Value.ShouldBe(42);
        result.Value.GetType().ShouldBe(typeof(int));
    }

    [Fact]
    public void Should_configure_options_with_known_types_registry_and_existing_converter()
    {
        var stub = new StubVariableQueryArgumentConverter();
        var options = new JsonSerializerOptions();
        options.Converters.Add(stub);

        options.ConfigureRemoteLinq(KnownTypesRegistry.Default);
        options.ConfigureRemoteLinq();

        // a converter already registered for the variable query argument must be retained, not replaced.
        options.Converters.ShouldContain(stub);
        options.Converters.Count(static x => x.CanConvert(typeof(VariableArgument))).ShouldBe(1);
        options.Converters.Count(static x => x.CanConvert(typeof(VariableArgumentList))).ShouldBe(1);
    }
}
