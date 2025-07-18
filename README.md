# MCP and IChatClient test extensions for xunit v3

The `TestBucket.AI.Xunit` package provides helpers for writing integration tests related to IChatClient and Model Context Protocol (MCP) servers using xUnit v3. 

## Getting Started

### Add Package Reference

- Ensure your test project references the following NuGet package: `TestBucket.AI.Xunit`

## Testing an MCP server

Use `McpClientFactory` (from the official `ModelContextProtocol` library) to create an `IMcpClient` instance. 
Typically, you will need to provide a transport (e.g., `SseClientTransport`) and authentication headers (if needed).

### Example

```csharp
using ModelContextProtocol.Client;
using TestBucket.AI.Xunit;

[Fact]
public async Task ExpectedTextFoundInSuccessfulResponse_AfterInvokingNavigateTool_WithCorrectArguments()
{
    // Arrange

    // For a complete example refer to tests/TestBucket.AI.PlaywrightMcpIntegrationTests/PlaywrightIntegrationTests.cs
    var client = await CreatePlaywrightMcpClientAsync();
    string url = "https://github.com/microsoft/playwright-mcp";
    var arguments = new ToolArgumentBuilder()
        .WithArgument("url", url)
        .Build();

    // Act: Call the tool
    var response = await client.TestCallToolAsync("browser_navigate", arguments);

    // Assert
    response.ShouldBeSuccess();
    response.ShouldHaveContent(content =>
    {
        content.ShouldContain("### Ran Playwright code");
        content.ShouldContain($"await page.goto('{url}');");
    });
}
```


## Benchmarking models

To benchmark models, you can use the `TestBucket.AI.Xunit` package to create tests that measure the performance (accuracy) of different models. 
The package provides a way to instrument calls to models and record metrics such as the accuracy invoking the correct tools with the correct arguments.

> As tools are added to a product, the selection by the LLM may be impacted especially if the tools are similar. The benchmarking model can be used to validate that specific prompts result in accurate calls to the correct tool.

### Benchmarking example with verification of tool invokation result

```csharp
foreach (string model in new string[] { "llama3.1:8b", "mistral-nemo:12b" })
{
    IChatClient client = InstrumentationChatClientFactory.Create(...);
    var benchmarkResult = await client.BencharkAsync("Add 3 and 6", iterations:2, (iterationResult) =>
    {
        iterationResult.ShouldBeSuccess();
        iterationResult.ContainsFunctionCall("Add");
    });

    // Write summary
    TestContext.Current.TestOutputHelper?.WriteLine($"Model: {model}, Passrate={benchmarkResult.Passrate}");

    // Write exceptions
    foreach(var exception in benchmarkResult.Exceptions)
    {
        TestContext.Current.TestOutputHelper?.WriteLine(exception.ToString());
    }
}
```

### Benchmarking results in xunit results XML

Details are added to the xunit results XML file, which can be used for further analysis, debugging or monitoring trends.

> Note that `IChatClient` should be created with both instrumentation and functional calling enabled in order to record metrics.
If you are not using the built-in `InstrumentationChatClientFactory` class to create a `IChatClient`, refer to that implementation to setup the `IChatClient` pipeline accordingly.


```xml
<attachments>
    <attachment name="AIUserPrompt">
        <![CDATA[ Add 3 and 6 ]]>
    </attachment>
    <attachment name="metric:llama3.1_8b:passrate">
        <![CDATA[ 100%@1749793857501 ]]>
    </attachment>
    <attachment name="metric:mistral-nemo_12b:passrate">
        <![CDATA[ 100%@1749793862549 ]]>
    </attachment>
</attachments>
```

## Verifying that the correct tool is called from a user-prompt

When adding new tools to your MCP server, it is possible that the tool selection breaks. The tool selection can be tested by calling TestGetResponseAsync and
examining the result which will contain information about what tools were called as well as additional diagnostics data.

> This example uses OllamaFixture to create an Ollama test container (using Testcontainers.Ollama)

```csharp
[EnrichedTest]
[IntegrationTest]
public class CalculatorToolTests(OllamaFixture Ollama) : IClassFixture<OllamaFixture>
{ 
    [Theory]
    [InlineData("llama3.1:8b")]
    public async Task CallSubtractTool_WithSimplePrompt_CorrectToolIsInvoked(string model)
    {
        // Arrange
        IChatClient chatClient = await CreateInstrumentedChatClientAsync(model);

        // Act
        InstrumentationTestResult result = await chatClient.TestGetResponseAsync("Subtract 5 from 19");

        // Assert
        result.ShouldBeSuccess();
        result.ContainsFunctionCall("Subtract", 1)
            .WithArgument("a", 19)
            .WithArgument("b", 5);
    }

    private async Task<IChatClient> CreateInstrumentedChatClientAsync(string model)
    {
        var toolAssembly = typeof(CalculatorMcp).Assembly;
        var chatClient = await Ollama.CreateChatClientAsync(model,
            configureServices: (services) =>
            {
                // Add any services required by the tools
                services.AddSingleton<ICalculator, Calculator>();
            },
            configureTools: (tools) =>
            {
                // Add McpServerTools from the assembly
                // Note: This scans the assembly for classes defining tools using the [McpServerToolType] attribute
                tools.AddMcpServerToolsFromAssembly(toolAssembly);
            });
        return chatClient;
    }
}
```

## Rich reports

When generating unit test reports, the `TestBucket.AI.Xunit` package provides additional details and metrics.

### Example of xunit xml report

```xml
<attachments>
    <attachment name="AIUserPrompt">
        <![CDATA[ Add 3 and 6 ]]>
    </attachment>
    <attachment name="AIModelName">
        <![CDATA[ llama3.1:8b ]]>
    </attachment>
    <attachment name="AIProviderName">
        <![CDATA[ ollama ]]>
    </attachment>
    <attachment name="AIProviderVersion">
        <![CDATA[ 0.6.6 ]]>
    </attachment>
    <attachment name="metric:testbucket.ai:input_token_count">
        <![CDATA[ 325tokens@1749786387459 ]]>
    </attachment>
    <attachment name="metric:testbucket.ai:output_token_count">
        <![CDATA[ 36tokens@1749786387464 ]]>
    </attachment>
    <attachment name="metric:testbucket.ai:total_token_count">
        <![CDATA[ 361tokens@1749786387464 ]]>
    </attachment>
    <attachment name="metric:xunit:test-duration">
        <![CDATA[ 35715.1986ms@1749786387472 ]]>
    </attachment>
    <attachment name="TestDescription">
        <![CDATA[ # TestBucket.McpTests.OllamaIntegrationTests.Llama3ToolInstrumentationTests.CallAddTool_WithTwoTools_CorrectToolIsInvoked(System.String) ## Summary Verifies that the correct tool is invoked when multiple tools are available ## Source | Assembly | Class | Method | | -------- | ----- | ------ | | TestBucket.AI.OllamaIntegrationTests | TestBucket.McpTests.OllamaIntegrationTests.Llama3ToolInstrumentationTests | CallAddTool_WithTwoTools_CorrectToolIsInvoked | ### Parameters | Name | Summary | | -------- | ------------------- | | model | | ]]>
    </attachment>
</attachments>
```

> Note: Test description is extracted from the xmldoc, and requires setting GenerateDocumentationFile to true in the .csproj file.
```xml
<PropertyGroup>
	<GenerateDocumentationFile>true</GenerateDocumentationFile>
</PropertyGroup>
```
