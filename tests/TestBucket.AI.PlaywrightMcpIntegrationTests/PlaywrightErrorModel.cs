using System.Text.Json.Serialization;

namespace TestBucket.AI.PlaywrightMcpIntegrationTests;

/// <summary>
/// Represents an error model as returned from the Playwright MCP server.
/// </summary>
public class PlaywrightErrorModel
{
    /// <summary>
    /// Gets or sets the error code.
    /// </summary>
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    /// <summary>
    /// Gets or sets the expected value.
    /// </summary>
    [JsonPropertyName("expected")]
    public string? Expected { get; set; }

    /// <summary>
    /// Gets or sets the received value.
    /// </summary>
    [JsonPropertyName("received")]
    public string? Received { get; set; }

    /// <summary>
    /// Gets or sets the path associated with the error.
    /// </summary>
    [JsonPropertyName("path")]
    public string[]? Path { get; set; }

    /// <summary>
    /// Gets or sets the error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string? Message { get; set; }
}