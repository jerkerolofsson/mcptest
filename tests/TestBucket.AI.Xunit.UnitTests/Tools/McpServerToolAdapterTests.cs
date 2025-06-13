using ModelContextProtocol.Server;

using System.ComponentModel;
using System.Text.Json;

using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.UnitTests.Tools
{
    /// <summary>
    /// Contains unit tests for the <see cref="McpServerToolAdapter"/> class.
    /// </summary>
    [UnitTest]
    [EnrichedTest]
    [FunctionalTest]
    [Feature("Tools")]
    [Component("McpServerToolAdapter")]
    public class McpServerToolAdapterTests
    {
        /// <summary>
        /// Verifies that when the <see cref="McpServerToolAdapter"/> is constructed with a method
        /// lacking the <c>McpServerTool</c> attribute, the name and description are set from the method name and <see cref="DescriptionAttribute"/>.
        /// </summary>
        [Fact]
        public void ToolMetadata_WithoutMcpServerToolAttribute_IsSetFromMethodNameAndDescription()
        {
            var adapter = new McpServerToolAdapter(AddWithoutMcpAttribute);

            // Name should default to method name
            Assert.Equal(nameof(AddWithoutMcpAttribute), adapter.Name);
            Assert.Equal(nameof(AddWithoutMcpAttribute), adapter.AIFunction.Name);

            // Description should come from [Description] attribute
            Assert.Contains("Adds two numbers", adapter.Description);
            Assert.Contains("Adds two numbers", adapter.AIFunction.Description);
        }

        /// <summary>
        /// Verifies that the <see cref="McpServerToolAdapter"/> correctly sets the name and description
        /// from the <c>McpServerTool</c> and <see cref="DescriptionAttribute"/> attributes.
        /// </summary>
        [Fact]
        public void ToolMetadata_IsSetCorrectly()
        {
            var adapter = new McpServerToolAdapter(AddInstance);
            Assert.Equal("Add", adapter.Name);
            Assert.Equal("Add", adapter.AIFunction.Name);
            Assert.Contains("Adds two numbers", adapter.Description);
            Assert.Contains("Adds two numbers", adapter.AIFunction.Description);
        }

        /// <summary>
        /// Verifies that the wrapped delegate in <see cref="McpServerToolAdapter"/> returns the correct value
        /// when invoking an instance method without arguments, returning an integer.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
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
        /// Verifies that the wrapped delegate in <see cref="McpServerToolAdapter"/> returns the correct value
        /// when invoking a static method without arguments, returning an integer.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
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
        /// Adds two numbers using a static method.
        /// </summary>
        /// <param name="a">The first number to add.</param>
        /// <param name="b">The second number to add.</param>
        /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
        [McpServerTool(Name = "Add"), Description("Adds two numbers")]
        public static int AddStatic(int a, int b) => a + b;

        /// <summary>
        /// Adds two numbers using an instance method.
        /// </summary>
        /// <param name="a">The first number to add.</param>
        /// <param name="b">The second number to add.</param>
        /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
        [McpServerTool(Name = "Add"), Description("Adds two numbers")]
        public int AddInstance(int a, int b) => a + b;

        /// <summary>
        /// Adds two numbers using an instance method without the <c>McpServerTool</c> attribute.
        /// </summary>
        /// <param name="a">The first number to add.</param>
        /// <param name="b">The second number to add.</param>
        /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
        [Description("Adds two numbers")]
        public int AddWithoutMcpAttribute(int a, int b) => a + b;
    }
}