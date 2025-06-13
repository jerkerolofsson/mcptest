using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;

namespace TestBucket.AI.Xunit.Tools;

/// <summary>
/// Extensions that combine IChatClient with Tools to ensure that the correct tool is selected
/// </summary>
public static class IChatClientWithToolsExtensions
{
    /// <summary>
    /// Gets a response from the chat client using the provided chat messages and tools.
    /// The tools are specified as a list of MethodInfo, which allows for easy integration with MCP server tools.
    /// </summary>
    /// <param name="chatClient"></param>
    /// <param name="chatMessages"></param>
    /// <param name="tools"></param>
    /// <param name="configureOptions">Callback that allows for additional configuration. Before this method is called the Tools property will be assigned</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<InstrumentationTestResult> GetResponseAsync(
        this IChatClient chatClient, 
        IEnumerable<ChatMessage> chatMessages, 
        IList<IAIFunctionAdapter> tools, 
        Action<ChatOptions>? configureOptions = null,
        CancellationToken cancellationToken = default)
    {
        ChatOptions options = new ChatOptions
        {
            Tools = [..tools.Select(x=>x.AIFunction)],
        };

        if(configureOptions is not null)
        {
            configureOptions(options);
        }

        var chatResponse = await chatClient.GetResponseAsync(chatMessages, options, cancellationToken);
        var result = chatClient.GetRequiredService<InstrumentationTestResult>();

        // Add to the xunit report
        TestContext.Current.AttachInstrumentationTestResult(result);

        return result;

    }
}
