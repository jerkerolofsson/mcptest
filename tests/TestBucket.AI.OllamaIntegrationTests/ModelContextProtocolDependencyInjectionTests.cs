using McpCalculator;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit;
using TestBucket.AI.Xunit.Ollama;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.OllamaIntegrationTests;

/// <summary>
/// Tests using a ModelContextProtocol tool that requires injected services
/// </summary>
/// <param name="Ollama"></param>
[EnrichedTest]
[IntegrationTest]
public class ModelContextProtocolDependencyInjectionTests(OllamaFixture Ollama)
{
    /// <summary>
    /// Verifies that the Add tool is invoked when multiple tools are available
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("llama3.1:8b")]
    public async Task CallAddTool_WithCalculatorMcpToolFromAssembly_CorrectToolIsInvoked(string model)
    {
        // Arrange
        IChatClient chatClient = await CalculatorUtils.CreateInstrumentedChatClientAsync(Ollama, model);

        // Act
        var result = await chatClient.TestGetResponseAsync("Add 3 and 6");

        // Assert
        result.ShouldBeSuccess();
        result.ContainsFunctionCall("Add", 1);
    }

    /// <summary>
    /// Verifies that the Subtract tool is invoked when multiple tools are available
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    [Theory]
    [InlineData("llama3.1:8b")]
    public async Task CallSubtractTool_WithCalculatorMcpToolFromAssembly_CorrectToolIsInvoked(string model)
    {
        // Arrange
        IChatClient chatClient = await CalculatorUtils.CreateInstrumentedChatClientAsync(Ollama, model);

        // Act
        var result = await chatClient.TestGetResponseAsync("Subtract 5 from 19");

        // Assert
        result.ShouldBeSuccess();
        result.ContainsFunctionCall("Subtract", 1)
            .WithArgument("a", 19)
            .WithArgument("b", 5);
    }


}
