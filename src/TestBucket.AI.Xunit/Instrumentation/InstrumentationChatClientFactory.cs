using System.Diagnostics;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.Instrumentation;
public class InstrumentationChatClientFactory
{
    /// <summary>
    /// Creates an <see cref="IChatClient"/> that uses the <see cref="InstrumentationChatClient"/> to instrument the chat client.
    /// 
    /// Adds an InstrumentationTestResult to the service provider, which can be used to verify the test results.
    /// </summary>
    /// <param name="client"></param>
    /// <returns></returns>
    public static IChatClient Create(IChatClient client, 
        Action<ServiceCollection>? configureServices = null,
        Action<IToolConfigurator>? toolConfigurator = null)
    {
        ArgumentNullException.ThrowIfNull(client);

        var activitySource = new ActivitySource(InstrumentationConstants.ActivitySourceName);

        var toolConfiguratorImpl = new ToolConfigurator();

        var services = new ServiceCollection();
        services.AddSingleton<InstrumentationTestResult>();
        services.AddSingleton(activitySource);
        services.AddSingleton(toolConfiguratorImpl);

        if (configureServices is not null)
        {
            configureServices(services);
        }

        var serviceProvider = services.BuildServiceProvider();

        toolConfiguratorImpl.Services = serviceProvider;
        if (toolConfigurator is not null)
        {
            toolConfigurator(toolConfiguratorImpl);
        }

        return new ChatClientBuilder(client)
            .UseInstrumentationChatClient()
            .UseFunctionInvocation()

            // This allows the chat clients to resolve services from the service provider even if the 
            // final IChatClient does not use the service provider directly.
            .UseServiceProvidingChatClient()
            .Build(serviceProvider);
    }
}
