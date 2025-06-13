using McpCalculator;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit;
using TestBucket.AI.Xunit.Ollama;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.OllamaIntegrationTests;

[EnrichedTest]
[IntegrationTest]
public class ModelContextProtocolDepdencyInjectionTests(OllamaFixture Ollama) : IClassFixture<OllamaFixture>
{
    /// <summary>
    /// Verifies that the correct tool is invoked when multiple tools are available
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("llama3.1:8b")]
    public async Task CallAddTool_WithCalculatorMcpToolFromAssembly_CorrectToolIsInvoked(string model)
    {
        // Arrange
        IChatClient chatClient = await CreateInstrumentedChatClientAsync(model);

        // Act
        var message = new ChatMessage(ChatRole.User, "Add 3 and 6");
        var result = await chatClient.TestGetResponseAsync([message], cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccess();
        result.ContainsFunctionCall("Add", 1);
    }

    /// <summary>
    /// Verifies that the correct tool is invoked when multiple tools are available
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("llama3.1:8b")]
    public async Task CallSubtractTool_WithCalculatorMcpToolFromAssembly_CorrectToolIsInvoked(string model)
    {
        // Arrange
        IChatClient chatClient = await CreateInstrumentedChatClientAsync(model);


        // Act
        var message = new ChatMessage(ChatRole.User, "Subtract 5 from 19");
        var result = await chatClient.TestGetResponseAsync([message], cancellationToken: TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeSuccess();
        result.ContainsFunctionCall("Subtract", 1);
    }


    private async Task<IChatClient> CreateInstrumentedChatClientAsync(string model)
    {
        var toolAssembly = typeof(CalculatorMcp).Assembly;
        var chatClient = await Ollama.CreateChatClientAsync(model,
            configureServices: (services) =>
            {
                services.AddSingleton<ICalculator, Calculator>();
            },
            configureTools: (tools) =>
            {
                tools.AddMcpServerToolsFromAssembly(toolAssembly);
            });
        return chatClient;
    }

}
