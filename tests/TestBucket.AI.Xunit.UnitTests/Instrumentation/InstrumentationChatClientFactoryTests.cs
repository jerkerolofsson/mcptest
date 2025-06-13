using System.Diagnostics;

using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;

namespace TestBucket.AI.Xunit.UnitTests.Instrumentation;

/// <summary>
/// Tests for InstrumentationChatClientFactory
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Instrumentation")]
[Component("InstrumentationChatClientFactory")]
public class InstrumentationChatClientFactoryTests
{
    /// <summary>
    /// Verifies that the InstrumentationChatClientFactory adds an ActivitySource to the service provider used by the IChatClient pipeline
    /// 
    /// This ActivitySource is used to provide metrics and data to the test reports
    /// </summary>
    [Fact]
    public void Create_ActivitySourceAddedToServicesAndHasCorrectName()
    {
        // Arrange
        var innerClient = NSubstitute.Substitute.For<IChatClient>();

        // Act
        var client = InstrumentationChatClientFactory.Create(innerClient);

        // Assert
        Assert.NotNull(client);
        var activitySource = client.GetRequiredService<ActivitySource>();
        Assert.Equal(InstrumentationConstants.ActivitySourceName, activitySource.Name);
    }
}
