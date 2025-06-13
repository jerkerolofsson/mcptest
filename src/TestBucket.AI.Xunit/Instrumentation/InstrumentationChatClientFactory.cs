using System.Diagnostics;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

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
    public static IChatClient Create(IChatClient client)
    {
        ArgumentNullException.ThrowIfNull(client);

        var activitySource = new ActivitySource(InstrumentationConstants.ActivitySourceName);

        var services = new ServiceCollection();
        services.AddSingleton<InstrumentationTestResult>();
        services.AddSingleton(activitySource);
        var serviceProvider = services.BuildServiceProvider();

        return new ChatClientBuilder(client)
            .UseInstrumentationChatClient()
            .UseFunctionInvocation()

            // This allows the chat clients to resolve services from the service provider even if the 
            // final IChatClient does not use the service provider directly.
            .UseServiceProvidingChatClient()
            .Build(serviceProvider);
    }
}
