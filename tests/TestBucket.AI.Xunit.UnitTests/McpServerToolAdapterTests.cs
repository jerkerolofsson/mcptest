using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.UnitTests
{
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Component("McpServerToolAdapter")]
    public class McpServerToolAdapterTests
    {
        /// <summary>
        /// Verifies that the wrapped delegate in McpServerToolAdapter returns the correct value
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async ValueTask InvokeInstanceAIFunction_WithoutArgumentsReturningInt_CorrectValueIsReturned()
        {
            var adapter = new McpServerToolAdapter(AddInstance);
            var args = new Dictionary<string, object?>() { { "a", 22 }, { "b", 20 } };
            var result = await adapter.AIFunction.InvokeAsync(new Microsoft.Extensions.AI.AIFunctionArguments(args), TestContext.Current.CancellationToken);
            if (result is JsonElement jsonElement)
            {
                Assert.Equal(JsonValueKind.Number, jsonElement.ValueKind);
                Assert.Equal(42, jsonElement.GetInt32());
            }
            else
            {
                Assert.Fail("Expected the return value from tool invokation to be a JsonElement");
            }
        }

        /// <summary>
        /// Verifies that the wrapped delegate in McpServerToolAdapter returns the correct value
        /// </summary>
        /// <returns></returns>
        [Fact]
        public async ValueTask InvokeStaticAIFunction_WithoutArgumentsReturningInt_CorrectValueIsReturned()
        {
            var adapter = new McpServerToolAdapter(AddStatic);
            var args = new Dictionary<string, object?>() { { "a", 22 }, { "b", 20 } };
            var result = await adapter.AIFunction.InvokeAsync(new Microsoft.Extensions.AI.AIFunctionArguments(args), TestContext.Current.CancellationToken);
            if (result is JsonElement jsonElement)
            {
                Assert.Equal(JsonValueKind.Number, jsonElement.ValueKind);
                Assert.Equal(42, jsonElement.GetInt32());
            }
            else
            {
                Assert.Fail("Expected the return value from tool invokation to be a JsonElement");
            }
        }

        /// <summary>
        /// Adds two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [McpServerTool(Name = "Add"), Description("Adds two numbers")]
        public static int AddStatic(int a, int b) => a + b;

        /// <summary>
        /// Adds two numbers
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        [McpServerTool(Name = "Add"), Description("Adds two numbers")]
        public int AddInstance(int a, int b) => a + b;
    }
}
