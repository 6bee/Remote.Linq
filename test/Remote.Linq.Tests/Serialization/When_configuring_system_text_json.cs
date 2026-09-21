// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Remote.Linq.Text.Json.Converters;
using System.Text.Json;
using VariableArgument = Remote.Linq.DynamicQuery.VariableQueryArgument;
using VariableArgumentList = Remote.Linq.DynamicQuery.VariableQueryArgumentList;

public class When_configuring_system_text_json
{
    [Fact]
    public void Should_register_remote_linq_converters_only_once()
    {
        var options = new JsonSerializerOptions();

        options.ConfigureRemoteLinq();
        var converterCount = options.Converters.Count;
        options.ConfigureRemoteLinq();

        options.Converters.Count.ShouldBe(converterCount);
        options.Converters.Any(converter => converter.CanConvert(typeof(VariableArgument))).ShouldBeTrue();
        options.Converters.Any(converter => converter.CanConvert(typeof(VariableArgumentList))).ShouldBeTrue();
    }

    [Fact]
    public void Should_roundtrip_variable_query_arguments_with_values()
    {
        var options = new JsonSerializerOptions().ConfigureRemoteLinq();
        var argument = new VariableArgument
        {
            Type = typeof(int).AsTypeInfo(),
            Value = 42,
        };
        var argumentResult = JsonSerializer.Deserialize<VariableArgument>(JsonSerializer.Serialize(argument, options), options);

        argumentResult.Type.ToType().ShouldBe(typeof(int));
        argumentResult.Value.ShouldBe(42);
    }

    [Fact]
    public void Should_create_default_variable_query_argument_converters()
    {
        new VariableQueryArgumentConverter().CanConvert(typeof(VariableArgument)).ShouldBeTrue();
        new VariableQueryArgumentListConverter().CanConvert(typeof(VariableArgumentList)).ShouldBeTrue();
    }
}
