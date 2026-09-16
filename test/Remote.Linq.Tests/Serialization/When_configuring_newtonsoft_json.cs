// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Remote.Linq.Tests.Serialization;

using global::Newtonsoft.Json;
using VariableArgument = Remote.Linq.DynamicQuery.VariableQueryArgument;

public class When_configuring_newtonsoft_json
{
    [Fact]
    public void Should_configure_settings_idempotently_and_create_remote_configuration()
    {
        var settings = new JsonSerializerSettings();

        settings.ConfigureRemoteLinq();
        settings.ConfigureRemoteLinq();

        settings.ContractResolver.ShouldBeOfType<Remote.Linq.Newtonsoft.Json.ContractResolvers.RemoteLinqContractResolver>();
        settings.CreateRemoteLinqConfiguration().ShouldBeOfType<RemoteLinqJsonSerializerSettings>();
    }

    [Fact]
    public void Should_roundtrip_variable_query_arguments()
    {
        var settings = new JsonSerializerSettings().ConfigureRemoteLinq();
        var argument = new VariableArgument(42, typeof(int));

        var argumentResult = JsonConvert.DeserializeObject<VariableArgument>(JsonConvert.SerializeObject(argument, settings), settings);

        argumentResult.Type.ToType().ShouldBe(typeof(int));
        argumentResult.Value.ShouldBe(42);
    }
}
