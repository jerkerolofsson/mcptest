using ModelContextProtocol.Client;
using TestBucket.AI.Xunit;
using TestBucket.Traits.Xunit;

namespace TestBucket.AI.PlaywrightMcpIntegrationTests;

/// <summary>
/// These tests assume that a playwright MCP server is running on localhost, e.g. 
/// 
/// npx @playwright/mcp@latest --isolated --port 8931
/// 
/// </summary>
public class PlaywrightIntegrationTests
{
    private static readonly Uri PlaywrightMcpUrl = new Uri("http://localhost:8930");
    private const string PlaywrightNavigateTool = "browser_navigate";

    /// <summary>
    /// Creates an MCP client that uses the SSE transport to connect to the Playwright MCP server running on localhost.
    /// </summary>
    /// <returns></returns>
    private async Task<IMcpClient> CreatePlaywrightMcpClientAsync()
    {
        var transport = new SseClientTransport(new SseClientTransportOptions { Endpoint = PlaywrightMcpUrl });
        var client = await McpClientFactory.CreateAsync(transport, cancellationToken: TestContext.Current.CancellationToken);
        return client;
    }


    /// <summary>
    /// Verifies that the tool call "browser_navigate" can be invoked with the correct arguments and that the response contains 
    /// the expected javascript code for navigation which was executed by the playwright server.
    /// </summary>
    /// <returns></returns>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest()]
    [Feature("Playwright MCP")]
    [Fact]
    public async Task CorrectErrorInResponse_AfterInvokingNavigateTool_WithMissingArguments()
    {
        // Arrange
        var client = await CreatePlaywrightMcpClientAsync();
        var arguments = new ToolArgumentBuilder()
            .Build();

        // Act: Call the tool
        var response = await client.TestCallToolAsync(PlaywrightNavigateTool, arguments);

        // Assert
        response.ShouldBeError();
        response.ShouldHaveContent(content =>
        {
            // This is the text response returned from Playwright MCP
            // [
            //    {
            //        "code": "invalid_type",
            //        "expected": "string",
            //        "received": "undefined",
            //        "path": [
            //            "url"
            //        ],
            //        "message": "Required"
            //    }
            //]
            content.AsJsonContent<PlaywrightErrorModel[]>(errors =>
            {
                Assert.NotNull(errors);
                Assert.Single(errors);

                var error = errors[0];
                Assert.Equal("invalid_type", error.Code);
                Assert.Equal("string", error.Expected);
                Assert.Equal("undefined", error.Received);
                Assert.Equal("Required", error.Message);
                Assert.NotNull(error.Path);
                Assert.Single(error.Path);
                Assert.Equal("url", error.Path[0]);
            });
        });
    }

    /// <summary>
    /// Verifies that the tool call "browser_navigate" can be invoked with the correct arguments and that the response contains 
    /// the expected javascript code for navigation which was executed by the playwright server.
    /// </summary>
    /// <returns></returns>
    [IntegrationTest]
    [EnrichedTest]
    [FunctionalTest()]
    [Feature("Playwright MCP")]
    [Fact]
    public async Task ExpectedTextFoundInSuccessfulResponse_AfterInvokingNavigateTool_WithCorrectArguments()
    {
        // Arrange
        var client = await CreatePlaywrightMcpClientAsync();
        string url = "https://github.com/microsoft/playwright-mcp";
        var arguments = new ToolArgumentBuilder()
            .WithArgument("url", url)
            .Build();

        // Act: Call the tool
        var response = await client.TestCallToolAsync(PlaywrightNavigateTool, arguments);

        // Assert
        response.ShouldBeSuccess();
        response.ShouldHaveContent(content =>
        {
            content.ShouldContain("### Ran Playwright code");
            content.ShouldContain($"await page.goto('{url}');");
        });
    }
}
