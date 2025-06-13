using ModelContextProtocol.Protocol;

namespace TestBucket.AI.Xunit;

public static class CallToolResponseExtensions
{
    public static void ShouldBeSuccess(this CallToolResponse toolResponse)
    {
        Assert.False(toolResponse.IsError, "CallToolResponse.IsError is true");
    }
}
