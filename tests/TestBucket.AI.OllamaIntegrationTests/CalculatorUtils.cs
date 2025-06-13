using McpCalculator;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit.Ollama;

namespace TestBucket.AI.OllamaIntegrationTests;
internal class CalculatorUtils
{

    internal static async Task<IChatClient> CreateInstrumentedChatClientAsync(OllamaFixture ollama, string model)
    {
        var toolAssembly = typeof(CalculatorMcp).Assembly;
        var chatClient = await ollama.CreateChatClientAsync(model,
            configureServices: (services) =>
            {
                // Add services required by the tools
                services.AddSingleton<ICalculator, Calculator>();
            },
            configureTools: (tools) =>
            {
                // Add McpServerTools from the assembly
                tools.AddMcpServerToolsFromAssembly(toolAssembly);
            });
        return chatClient;
    }
}
