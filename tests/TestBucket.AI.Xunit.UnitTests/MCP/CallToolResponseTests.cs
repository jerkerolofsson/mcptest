using System;

using Microsoft.Extensions.AI;

using ModelContextProtocol.Protocol;

using Xunit;
using Xunit.Sdk;

namespace TestBucket.AI.Xunit.UnitTests.MCP;

/// <summary>
/// Unit tests for the <see cref="CallToolResponse"/> class.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("MCP")]
[Component("CallToolResponse")]
public class CallToolResponseTests
{
    private Content SomeContent => new Content() {  Text = "Hello, MCP world!" };

    /// <summary>
    /// Verifies that <see cref="CallToolResponse.IsError"/> is false for a successful response.
    /// </summary>
    [Fact]
    public void ShouldBeSuccess_WhenIsErrorIsFalse_DoesNotThrow()
    {
        var response = new CallToolResponse
        {
            IsError = false,
            Content = [SomeContent]
        };

        var testResponse = new TestCallToolResponse(response);
        testResponse.ShouldBeSuccess();
    }

    /// <summary>
    /// Verifies that <see cref="CallToolResponse.IsError"/> is true causes ShouldBeSuccess to throw.
    /// </summary>
    [Fact]
    public void ShouldBeSuccess_WhenHaveContentAndIsErrorIsTrue_ThrowsFailException()
    {
        var response = new CallToolResponse
        {
            IsError = true,
            Content = [SomeContent]
        };

        var testResponse = new TestCallToolResponse(response);
        Assert.Throws<FailException>(() => testResponse.ShouldBeSuccess());
    }

    /// <summary>
    /// Verifies that <see cref="CallToolResponse.Content"/> is not empty for a valid response.
    /// </summary>
    [Fact]
    public void ShouldHaveContent_WhenContentIsNotEmpty_DoesNotThrow()
    {
        var response = new CallToolResponse
        {
            IsError = false,
            Content = [SomeContent]
        };

        var testResponse = new TestCallToolResponse(response);
        testResponse.ShouldHaveContent();
    }

    /// <summary>
    /// Verifies that <see cref="CallToolResponse.Content"/> is empty causes ShouldHaveContent to throw.
    /// </summary>
    [Fact]
    public void ShouldHaveContent_WhenContentIsNull_ThrowsArgumentNullException()
    {
        var response = new CallToolResponse
        {
            IsError = false,
            Content = null!
        };

        var testResponse = new TestCallToolResponse(response);
        Assert.Throws<ArgumentNullException>(() => testResponse.ShouldHaveContent());
    }
    /// <summary>
    /// Verifies that <see cref="CallToolResponse.Content"/> is empty causes ShouldHaveContent to throw.
    /// </summary>
    [Fact]
    public void ShouldHaveContent_WhenContentIsEmpty_Throws()
    {
        var response = new CallToolResponse
        {
            IsError = false,
            Content = []
        };

        var testResponse = new TestCallToolResponse(response);
        Assert.Throws<NotEmptyException>(() => testResponse.ShouldHaveContent());
    }
}