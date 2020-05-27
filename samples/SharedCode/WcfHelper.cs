// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

using System;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

/// <summary>
/// Common code used to set-up WCF configuration.
/// </summary>
internal static class WcfHelper
{
    public static ServiceHost CreateServiceHost<TService>()
        where TService : class
        => new ServiceHost(typeof(TService)).IncludeExceptionDetailInFaults();

    public static ServiceHost IncludeExceptionDetailInFaults(this ServiceHost wcfServiceHost)
    {
        wcfServiceHost.Description.Behaviors.OfType<ServiceDebugBehavior>().Single().IncludeExceptionDetailInFaults = true;
        return wcfServiceHost;
    }

    public static ServiceHost AddWsHttpEndpoint<TService>(this ServiceHost wcfServiceHost, string url = "")
    {
        wcfServiceHost.AddServiceEndpoint(typeof(TService), new WSHttpBinding(), url);
        return wcfServiceHost;
    }

    public static ServiceHost AddNetTcpEndpoint<TService>(this ServiceHost wcfServiceHost, string url = "")
    {
        wcfServiceHost.AddServiceEndpoint(typeof(TService), new NetTcpBinding(), url);
        return wcfServiceHost;
    }

    public static ServiceHost AddNetNamedPipeEndpoint<TService>(this ServiceHost wcfServiceHost, string url = "")
    {
        wcfServiceHost.AddServiceEndpoint(typeof(TService), new NetNamedPipeBinding(), url);
        return wcfServiceHost;
    }

    public static ServiceHost AddMexEndpoint(this ServiceHost wcfServiceHost)
    {
        var serviceMetadataBehavior = wcfServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() ?? new ServiceMetadataBehavior();
        serviceMetadataBehavior.HttpGetEnabled = true;
        serviceMetadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
        wcfServiceHost.Description.Behaviors.Add(serviceMetadataBehavior);
        wcfServiceHost.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexHttpBinding(), "mex");
        return wcfServiceHost;
    }

    public static ServiceHost OpenService(this ServiceHost wcfServiceHost)
    {
        wcfServiceHost.Open();
        return wcfServiceHost;
    }

    public static TBinding WithDebugSetting<TBinding>(this TBinding binding)
        where TBinding : Binding
    {
        binding.CloseTimeout = TimeSpan.FromMinutes(10);
        binding.ReceiveTimeout = TimeSpan.FromMinutes(10);
        binding.SendTimeout = TimeSpan.FromMinutes(10);
        return binding;
    }

    public static WcfServiceProxy<TService> CreateServiceProxy<TService>(this ChannelFactory<TService> channelFactory)
        => new WcfServiceProxy<TService>(channelFactory.CreateChannel());
}

internal sealed class WcfServiceProxy<TService> : IDisposable
{
    private readonly ChannelFactory<TService> _channelFactory;

    public WcfServiceProxy(Binding binding, string uri)
    {
        _channelFactory = new ChannelFactory<TService>(binding, uri);
        Service = _channelFactory.CreateChannel();
    }

    public WcfServiceProxy(TService service)
    {
        Service = service ?? throw new ArgumentNullException(nameof(service));
    }

    public TService Service { get; }

    public void Dispose()
    {
        if (Service is ICommunicationObject communicationObject)
        {
            if (communicationObject.State == CommunicationState.Faulted)
            {
                communicationObject.Abort();
            }
            else
            {
                communicationObject.Close();
            }
        }

        _channelFactory?.Close();
    }
}