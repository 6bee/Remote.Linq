// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using Aqua;
using global::Newtonsoft.Json;
using global::Newtonsoft.Json.Serialization;
using Remote.Linq.Expressions;
using Remote.Linq.Newtonsoft.Json.ContractResolvers;
using VariableArgument = Remote.Linq.DynamicQuery.VariableQueryArgument;

public class When_serializing_newtonsoft_expression_metadata
{
    private static JsonSerializerSettings Settings => new JsonSerializerSettings().ConfigureRemoteLinq();

    [Fact]
    public void Should_serialize_constant_with_value_type_differing_from_declared_type()
    {
        var constant = new ConstantExpression(42, typeof(object));

        var json = JsonConvert.SerializeObject(constant, Settings);

        json.ShouldContain("\"ValueType\":\"int32\"");
        json.ShouldContain("\"Name\":\"Object\"");
        json.ShouldContain("\"Value\":42");

        var result = JsonConvert.DeserializeObject<ConstantExpression>(json, Settings);

        result.Type.ToType().ShouldBe(typeof(object));
        result.Value.ShouldBe(42);
        result.Value.GetType().ShouldBe(typeof(int));
    }

    [Fact]
    public void Should_configure_settings_with_known_types_registry_and_existing_resolver()
    {
        var registry = KnownTypesRegistry.Default;
        var settings = new JsonSerializerSettings { ContractResolver = new DefaultContractResolver() };

        settings.ConfigureRemoteLinq(registry);
        settings.ConfigureRemoteLinq();

        settings.ContractResolver.ShouldBeOfType<RemoteLinqContractResolver>();

        // the second configuration call must not replace a working resolver with a broken one:
        // serialization of a variable query argument keeps working after both calls.
        var argument = new VariableArgument(42, typeof(int));
        var argumentResult = JsonConvert.DeserializeObject<VariableArgument>(JsonConvert.SerializeObject(argument, settings), settings);
        argumentResult.Type.ToType().ShouldBe(typeof(int));
        argumentResult.Value.ShouldBe(42);

        var configuration = settings.CreateRemoteLinqConfiguration(KnownTypesRegistry.Default);

        configuration.ShouldBeOfType<RemoteLinqJsonSerializerSettings>();
    }
}
