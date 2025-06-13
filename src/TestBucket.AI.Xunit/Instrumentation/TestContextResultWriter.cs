using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.Traits.Core.Metrics;
using TestBucket.Traits.Xunit;

namespace TestBucket.AI.Xunit.Instrumentation;

/// <summary>
/// Augments metrics and other details to the TestContext as attachments so it is included in the xml reports
/// when running tests with xUnit.
/// </summary>
internal static class TestContextResultWriter
{
    private const string MeterName = "TestBucket.AI.Xunit.Instrumentation";

    public static void AttachInstrumentationTestResult(this ITestContext testContext, InstrumentationTestResult result)
    {
        var userMessages = result.RequestMessages.Where(x => x.Role == ChatRole.User).SelectMany(x=>x.Contents);
        testContext.AddAttachmentIfNotExists(ResultTraitNames.LlmUserPrompt, ConvertAIMessagesToText(userMessages));

        if (result.InputTokenCount is not null)
        {
            testContext.AddMetric(new TestResultMetric(MeterName, "input-token-count", result.InputTokenCount.Value, "tokens"));
        }
        if (result.OutputTokenCount is not null)
        {
            testContext.AddMetric(new TestResultMetric(MeterName, "output-token-count", result.OutputTokenCount.Value, "tokens"));
        }
        if (result.TotalTokenCount is not null)
        {
            testContext.AddMetric(new TestResultMetric(MeterName, "total-token-count", result.TotalTokenCount.Value, "tokens"));
        }
    }

    private static string ConvertAIMessagesToText(IEnumerable<AIContent> messageContent)
    {
        var stringBuilder = new StringBuilder();

        foreach (var content in messageContent)
        {
            var raw = content.RawRepresentation?.ToString();
            if (raw is not null)
            {
                stringBuilder.AppendLine(raw);
            }
        }

        return stringBuilder.ToString();
    }
}
