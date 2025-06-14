﻿using Microsoft.Extensions.AI;

namespace TestBucket.AI.Xunit.Instrumentation;

/// <summary>
/// OllamaClient doesn't implement IServiceProvider, so we need to create a delegating client that does
/// </summary>
internal class ServiceProvidingClient : DelegatingChatClient
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceProvidingClient(IChatClient innerClient, IServiceProvider serviceProvider) : base(innerClient)
    {
        _serviceProvider = serviceProvider;
    }

    public override object? GetService(Type serviceType, object? serviceKey = null)
    {
        var service = InnerClient.GetService(serviceType, serviceKey);
        if(service is null)
        {
            service = _serviceProvider.GetService(serviceType);
        }
        return service;
    }
}
