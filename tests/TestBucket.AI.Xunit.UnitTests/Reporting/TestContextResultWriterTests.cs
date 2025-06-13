using Microsoft.Extensions.AI;

using NSubstitute;

using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.AI.Xunit.Reporting;

namespace TestBucket.AI.Xunit.UnitTests.Reporting;

/// <summary>
/// Contains unit tests for the <see cref="TestContextResultWriter"/> class.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Component("TestContextResultWriter")]
[Feature("Reporting")]
public class TestContextResultWriterTests
{
    /// <summary>
    /// Verifies that AttachInstrumentationTestResult attaches user prompt and model information as attachments.
    /// </summary>
    [Fact]
    public void AttachInstrumentationTestResult_AttachesUserPromptAndModelInfo()
    {
        // Arrange
        var testContext = Substitute.For<ITestContext>();
        var userMessage = new ChatMessage(ChatRole.User, new AIContent[] { new TextContent("Hello AI!") });
        var result = new InstrumentationTestResult
        {
            ModelName = "gpt-4",
            ProviderName = "OpenAI",
            ProviderVersion = "2024-06-01"
        };
        result.AddRequestMessages(new[] { userMessage });

        // Act
        testContext.AttachInstrumentationTestResult(result);

        // Assert
        testContext.Received().AddAttachmentIfNotExists(ResultTraitNames.AIUserPrompt, "Hello AI!");
        testContext.Received().AddAttachmentIfNotExists(ResultTraitNames.AIModelName, "gpt-4");
        testContext.Received().AddAttachmentIfNotExists(ResultTraitNames.AIProviderName, "OpenAI");
        testContext.Received().AddAttachmentIfNotExists(ResultTraitNames.AIProviderVersion, "2024-06-01");
    }

    /// <summary>
    /// Verifies that <see cref="TestContextResultWriter.ConvertAIMessagesToText"/> concatenates multiple <see cref="TextContent"/> objects into a single string.
    /// </summary>
    [Fact]
    public void ConvertAIMessagesToText_ConcatenatesTextContent()
    {
        // Arrange
        var contents = new AIContent[]
        {
            new TextContent("Hello, "),
            new TextContent("world!")
        };

        // Act
        var result = TestContextResultWriter.ConvertAIMessagesToText(contents);

        // Assert
        Assert.Equal("Hello, world!", result);
    }
}