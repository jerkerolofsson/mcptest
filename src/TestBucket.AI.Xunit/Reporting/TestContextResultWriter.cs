using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.Traits.Core.Metrics;
using TestBucket.Traits.Xunit;

namespace TestBucket.AI.Xunit.Reporting;

/// <summary>
/// Augments metrics and other details to the TestContext as attachments so it is included in the xml reports
/// when running tests with xUnit.
/// </summary>
internal static class TestContextResultWriter
{
    private const string MeterName = "testbucket.ai";

    public static void AttachInstrumentationTestResult(this ITestContext testContext, InstrumentationTestResult result)
    {
        var userMessages = result.RequestMessages.Where(x => x.Role == ChatRole.User).SelectMany(x=>x.Contents);
        testContext.AddAttachmentIfNotExists(ResultTraitNames.AIUserPrompt, ConvertAIMessagesToText(userMessages));

        if (result.ModelName is not null)
        {
            testContext.AddAttachmentIfNotExists(ResultTraitNames.AIModelName, result.ModelName);
        }
        if (result.ProviderName is not null)
        {
            testContext.AddAttachmentIfNotExists(ResultTraitNames.AIProviderName, result.ProviderName);
        }
        if (result.ProviderVersion is not null)
        {
            testContext.AddAttachmentIfNotExists(ResultTraitNames.AIProviderVersion, result.ProviderVersion);
        }

        if (result.InputTokenCount is not null)
        {
            testContext.AddMetric(new TestResultMetric(MeterName, "input_token_count", result.InputTokenCount.Value, "tokens"));
        }
        if (result.OutputTokenCount is not null)
        {
            testContext.AddMetric(new TestResultMetric(MeterName, "output_token_count", result.OutputTokenCount.Value, "tokens"));
        }
        if (result.TotalTokenCount is not null)
        {
            testContext.AddMetric(new TestResultMetric(MeterName, "total_token_count", result.TotalTokenCount.Value, "tokens"));
        }
    }

    internal static string ConvertAIMessagesToText(IEnumerable<AIContent> messageContent)
    {
        var stringBuilder = new StringBuilder();

        foreach (var content in messageContent)
        {
            if(content is TextContent textContent)
            {
                stringBuilder.Append(textContent.Text);
            }
            else
            {
                var raw = content.RawRepresentation?.ToString();
                if (raw is not null)
                {
                    stringBuilder.Append(raw);
                }
            }
        }

        return stringBuilder.ToString();
    }
}
