using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.AI.Xunit.Reporting;

namespace TestBucket.AI.Xunit.Tools;

/// <summary>
/// Extensions that combine IChatClient with Tools to ensure that the correct tool is selected
/// </summary>
public static class IChatClientWithToolsExtensions
{
    public static async Task<InstrumentationTestResult> TestGetResponseAsync(
    this IChatClient chatClient,
    string chatMessage,
    Action<ChatOptions>? configureOptions = null)
    {
        var message = new ChatMessage(ChatRole.User, chatMessage);
        return await TestGetResponseAsync(chatClient, [message], configureOptions, true, TestContext.Current.CancellationToken);
    }

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
    public static async Task<InstrumentationTestResult> TestGetResponseAsync(
        this IChatClient chatClient,
        IEnumerable<ChatMessage> chatMessages,
        Action<ChatOptions>? configureOptions = null,
        bool addToReport = true,
        CancellationToken cancellationToken = default)
    {
        ChatOptions options = new ChatOptions();

        var toolConfigurator = chatClient.GetRequiredService<ToolConfigurator>();
        var functions = toolConfigurator.Build();
        if (functions != null)
        {
            options.Tools = [..functions.Select(x=>x.AIFunction)];
        }

        if (configureOptions is not null)
        {
            configureOptions(options);
        }

        var chatResponse = await chatClient.GetResponseAsync(chatMessages, options, cancellationToken);
        var result = chatClient.GetRequiredService<InstrumentationTestResult>();

        if (addToReport)
        {
            // Add to the xunit report
            TestContext.Current.AttachInstrumentationTestResult(result);
        }

        return result;

    }
}
