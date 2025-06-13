# MCP and IChatClient test extensions for xunit v3

The `TestBucket.McpTest.Xunit` package provides helpers for writing integration tests related to IChatClient and Model Context Protocol (MCP) servers using xUnit v3. 

## Getting Started

Add Package Reference

- Ensure your test project references the following NuGet package: `TestBucket.AI.Xunit`

## Benchmarking models

To benchmark models, you can use the `TestBucket.McpTest.Xunit` package to create tests that measure the performance of different models. 
The package provides a way to instrument calls to models and record metrics such as the accuracy invoking the correct tools with the correct arguments.

### Benchmarking example without verification of result

```csharp
foreach (string model in new string[] { "llama3.1:8b", "mistral-nemo:12b" })
{
    IChatClient client = ...;
    var benchmarkResult = await client.BencharkAsync("Add 3 and 6", iterations:2, (iterationResult) =>
    {
        iterationResult.ShouldBeSuccess();
        iterationResult.ContainsFunctionCall("Add");
    });

    // Write summary
    TestContext.Current.TestOutputHelper?.WriteLine("Model: {ModelName}, Passrate={Passrate} ({IterationsPassed}/{IterationsStarted})", 
        model, 
        benchmarkResult.Passrate, // 0.0 - 100.0
        benchmarkResult.IterationsPassed,
        benchmarkResult.IterationsStarted);

    // Write exceptions
    foreach(var exception in benchmarkResult.Exceptions)
    {
        TestContext.Current.TestOutputHelper?.WriteLine(exception.ToString());
    }
}
```

### Benchmarking results in xunit results XML

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
        result.ContainsFunctionCall("Subtract", 1);
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

When generating unit test reports, the `TestBucket.McpTest.Xunit` package provides additional details and metrics.

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

## Testing an MCP server

It is designed to work with the `IMcpClient` interface and the `McpClientFactory` from the `ModelContextProtocol` library.

Use `McpClientFactory` to create an `IMcpClient` instance. Typically, you will need to provide a transport (e.g., `SseClientTransport`) and authentication headers (if needed).

### Example

```csharp
[Fact]
public async Task Should_Invoke_MyTool_Successfully()
{
    // Arrange: create your IMcpClient (using your factory or fixture)
    IMcpClient client = /* get or create your client, e.g. from a fixture */;

    // Tool name and arguments
    string toolName = "myTool";
    var arguments = new Dictionary<string, object?>
    {
        { "param1", "value1" },
        { "param2", 42 }
    };

    // Call the tool
    var response = await client.TestCallToolAsync(
        toolName,
        arguments
    );

    // Assert: check the response
    response.ShouldBeSuccess();
    response.ShouldHaveContent();
}
```