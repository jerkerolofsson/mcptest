using System.ComponentModel;

using Microsoft.Extensions.AI;

using ModelContextProtocol.Server;

using TestBucket.AI.Xunit;
using TestBucket.AI.Xunit.Ollama;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.McpTests.OllamaIntegrationTests
{
    /// <summary>
    /// Integration tests using ollama/llama3.1 with tools
    /// </summary>
    /// <param name="Ollama"></param>
    [EnrichedTest]
    [IntegrationTest]
    public class Llama3ToolInstrumentationTests(OllamaFixture Ollama) : IClassFixture<OllamaFixture>
    {
        /// <summary>
        /// Verifies that the correct tool is invoked when multiple tools are available
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Theory]
        [InlineData("llama3.1:8b")]
        public async Task CallAddTool_WithTwoTools_CorrectToolIsInvoked(string model)
        {
            // Arrange
            var chatClient = await Ollama.CreateChatClientAsync(model, configureTools: (tools) =>
            {
                tools.Add(Add);
                tools.Add(Subtract);
            });
           
            // Act
            var message = new ChatMessage(ChatRole.User, "Add 3 and 6");
            var result = await chatClient.TestGetResponseAsync([message], cancellationToken: TestContext.Current.CancellationToken);

            // Assert
            result.ShouldBeSuccess();
            result.ContainsFunctionCall("Add", 1);
        }

        /// <summary>
        /// Adds two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [McpServerTool(Name = "Add"), Description("Adds two numbers")]
        public static int Add(int a, int b) => a + b;

        /// <summary>
        /// Subtracts two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [McpServerTool(Name = "Subtract"), Description("Subtracts two numbers")]
        public static int Subtract(int a, int b) => a - b;
    }
}
