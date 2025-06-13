using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Reporting;
using TestBucket.Traits.Core.Metrics;
using TestBucket.Traits.Xunit;

namespace TestBucket.AI.Xunit.Benchmarking;

/// <summary>
/// Augments metrics and other details to the TestContext as attachments so it is included in the xml reports
/// when running tests with xUnit.
/// </summary>
internal static class TestContextBenchmarkResultWriter
{
    private const string MeterName = "testbucket.ai";

    /// <summary>
    /// Attaches benchmark results and metrics to the test context for reporting.
    /// </summary>
    /// <param name="testContext">The test context to augment.</param>
    /// <param name="benchmarkResult">The benchmark result containing metrics and instrumentation data.</param>
    public static void AttachBenchmarkResult(this ITestContext testContext, BenchmarkResult benchmarkResult)
    {
        string meterName = MeterName;

        // If there are instrumentation results, attach the first user prompt and use the model name as the meter name if available.
        if (benchmarkResult.InstrumentationTestResults.Count > 0)
        {
            var result = benchmarkResult.InstrumentationTestResults[0];
            var userMessages = result.RequestMessages.Where(x => x.Role == ChatRole.User).SelectMany(x => x.Contents);
            testContext.AddAttachmentIfNotExists(ResultTraitNames.AIUserPrompt, TestContextResultWriter.ConvertAIMessagesToText(userMessages));

            if (result.ModelName is not null)
            {
                meterName = result.ModelName.Replace(':', '_');
            }
        }

        // Add basic metrics: pass rate, duration, failures, and passes.
        testContext.AddMetric(new TestResultMetric(meterName, "passrate", benchmarkResult.Passrate * 100, "%"));
        testContext.AddMetric(new TestResultMetric(meterName, "duration", benchmarkResult.AccumulatedDuration.TotalSeconds, "s"));
        testContext.AddMetric(new TestResultMetric(meterName, "failures", benchmarkResult.IterationsFailed, ""));
        testContext.AddMetric(new TestResultMetric(meterName, "passed", benchmarkResult.IterationsPassed, ""));

        // Aggregate token counts from all instrumentation results.
        long outputTokenCount = benchmarkResult.InstrumentationTestResults.Sum(r => r.OutputTokenCount ?? 0);
        long inputTokenCount = benchmarkResult.InstrumentationTestResults.Sum(r => r.InputTokenCount ?? 0);
        long totalTokenCount = benchmarkResult.InstrumentationTestResults.Sum(r => r.TotalTokenCount ?? 0);

        // Add token count metrics if available.
        if (inputTokenCount > 0)
        {
            testContext.AddMetric(new TestResultMetric(meterName, "input_token_count", outputTokenCount, "tokens"));
        }
        if (outputTokenCount > 0)
        {
            testContext.AddMetric(new TestResultMetric(meterName, "output_token_count", outputTokenCount, "tokens"));
        }
        if (totalTokenCount > 0)
        {
            testContext.AddMetric(new TestResultMetric(meterName, "total_token_count", outputTokenCount, "tokens"));
        }
    }
}