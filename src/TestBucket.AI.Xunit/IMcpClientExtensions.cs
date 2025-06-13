using ModelContextProtocol;
using ModelContextProtocol.Client;
using System.Text.Json;
using Xunit;

namespace TestBucket.AI.Xunit;
public static class IMcpClientExtensions
{
    public static async ValueTask<TestCallToolResponse> TestCallToolAsync(this IMcpClient client, 
        string toolName,
        IReadOnlyDictionary<string, object?>? arguments = null,
        IProgress<ProgressNotificationValue>? progress = null,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        var result = await client.CallToolAsync(toolName, arguments, progress, jsonSerializerOptions, TestContext.Current.CancellationToken);
        return new TestCallToolResponse(result);
    }
}
