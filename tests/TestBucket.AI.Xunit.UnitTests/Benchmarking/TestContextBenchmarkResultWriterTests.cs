using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Benchmarking;
using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.AI.Xunit.Reporting;
using TestBucket.Traits.Core.Metrics;
using TestBucket.Traits.Xunit;

using Xunit;
using Xunit.Sdk;

namespace TestBucket.AI.Xunit.UnitTests.Benchmarking;

/// <summary>
/// Unit tests for <see cref="TestContextBenchmarkResultWriter"/>.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Benchmarking")]
[Component("Benchmarking")]
public class TestContextBenchmarkResultWriterTests
{
    /// <summary>
    /// Verifies that <see cref="TestContextBenchmarkResultWriter.AttachBenchmarkResult"/> attaches metrics and user prompt correctly.
    /// </summary>
    [Fact]
    public void AttachBenchmarkResult_AttachesMetricsAndPrompt()
    {
        // Arrange
        var testContext = new FakeTestContext();
        var parameters = new BenchmarkParameters { ChatMessages = Array.Empty<ChatMessage>() };
        var benchmarkResult = new BenchmarkResult(parameters)
        {
            IterationsPassed = 2,
            IterationsStarted = 3,
            IterationsFailed = 1,
            AccumulatedDuration = TimeSpan.FromSeconds(5)
        };

        var userMessage = new ChatMessage(ChatRole.User, [new TextContent("Hello world")]);
        var instrResult = new InstrumentationTestResult
        {
            ModelName = "test-model",
            InputTokenCount = 10,
            OutputTokenCount = 20,
            TotalTokenCount = 30
        };
        instrResult.AddRequestMessages([userMessage]);
        benchmarkResult.AddInstrumentationTestResult(instrResult);

        // Act
        testContext.AttachBenchmarkResult(benchmarkResult);

        // Assert
        Assert.Contains(testContext.Attachments, a => a.Key == ResultTraitNames.AIUserPrompt && a.Value.AsString() == "Hello world");

        // Metrics are prefixed with the model name
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "passrate" && m.Value.Equals(66.66666666666666));
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "duration" && m.Value.Equals(5.0));
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "failures" && m.Value.Equals(1));
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "passed" && m.Value.Equals(2));
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "input_token_count" && m.Value.Equals(20L));
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "output_token_count" && m.Value.Equals(20L));
        Assert.Contains(testContext.Metrics, m => m.MeterName == "test-model" && m.Name == "total_token_count" && m.Value.Equals(20L));
    }

    /// <summary>
    /// Verifies that <see cref="TestContextBenchmarkResultWriter.AttachBenchmarkResult"/> does not attach user prompt if no instrumentation results exist.
    /// </summary>
    [Fact]
    public void AttachBenchmarkResult_NoInstrumentationResults_DoesNotAttachPrompt()
    {
        // Arrange
        var testContext = new FakeTestContext();
        var parameters = new BenchmarkParameters { ChatMessages = Array.Empty<ChatMessage>() };
        var benchmarkResult = new BenchmarkResult(parameters);

        // Act
        testContext.AttachBenchmarkResult(benchmarkResult);

        // Assert
        Assert.DoesNotContain(testContext.Attachments, a => a.Key == ResultTraitNames.AIUserPrompt);
    }

    /// <summary>
    /// Verifies that <see cref="TestContextBenchmarkResultWriter.AttachBenchmarkResult"/> uses model name as meter name if available.
    /// </summary>
    [Fact]
    public void AttachBenchmarkResult_UsesModelNameAsMeterName()
    {
        // Arrange
        var testContext = new FakeTestContext();
        var parameters = new BenchmarkParameters { ChatMessages = Array.Empty<ChatMessage>() };
        var benchmarkResult = new BenchmarkResult(parameters)
        {
            IterationsPassed = 1,
            IterationsStarted = 1,
            IterationsFailed = 0,
            AccumulatedDuration = TimeSpan.FromSeconds(1)
        };

        var instrResult = new InstrumentationTestResult
        {
            ModelName = "my:model"
        };
        instrResult.AddRequestMessages([new ChatMessage(ChatRole.User, [new TextContent("Prompt")])]);
        benchmarkResult.AddInstrumentationTestResult(instrResult);

        // Act
        testContext.AttachBenchmarkResult(benchmarkResult);

        // Assert
        Assert.Contains(testContext.Metrics, m => m.MeterName == "my_model");
    }

    /// <summary>
    /// A simple fake implementation of <see cref="ITestContext"/> for testing.
    /// </summary>
    private class FakeTestContext : ITestContext
    {
        public Dictionary<string, TestAttachment> Attachments { get; } = new();
        public IEnumerable<TestResultMetric> Metrics
        {
            get
            { 
                foreach(var metricAttachment in Attachments.Where(a => a.Key.StartsWith("metric")))
                {
                    yield return MetricSerializer.Deserialize(metricAttachment.Key, metricAttachment.Value.AsString());
                }
            }
        }

        public CancellationToken CancellationToken => CancellationToken.None;

        public Dictionary<string, object?> KeyValueStorage => throw new NotImplementedException();

        public TestPipelineStage PipelineStage => throw new NotImplementedException();

        public ITest? Test => throw new NotImplementedException();

        public ITestAssembly? TestAssembly => throw new NotImplementedException();

        public TestEngineStatus? TestAssemblyStatus => throw new NotImplementedException();

        public ITestCase? TestCase => throw new NotImplementedException();

        public TestEngineStatus? TestCaseStatus => throw new NotImplementedException();

        public ITestClass? TestClass => throw new NotImplementedException();

        public object? TestClassInstance => throw new NotImplementedException();

        public TestEngineStatus? TestClassStatus => throw new NotImplementedException();

        public ITestCollection? TestCollection => throw new NotImplementedException();

        public TestEngineStatus? TestCollectionStatus => throw new NotImplementedException();

        public ITestMethod? TestMethod => throw new NotImplementedException();

        public TestEngineStatus? TestMethodStatus => throw new NotImplementedException();

        public ITestOutputHelper? TestOutputHelper => throw new NotImplementedException();

        public TestResultState? TestState => throw new NotImplementedException();

        public TestEngineStatus? TestStatus => throw new NotImplementedException();

        public IReadOnlyList<string>? Warnings => throw new NotImplementedException();

        IReadOnlyDictionary<string, TestAttachment>? ITestContext.Attachments => Attachments;

        public void AddAttachment(string name, string value)
        {
            Attachments[name] = TestAttachment.Create(value);
        }

        public void AddAttachment(string name, byte[] value, string mediaType = "application/octet-stream")
        {
            Attachments[name] = TestAttachment.Create(value, mediaType);
        }

        public void AddWarning(string message)
        {
            throw new NotImplementedException();
        }

        public void CancelCurrentTest()
        {
            throw new NotImplementedException();
        }

        public ValueTask<object?> GetFixture(Type fixtureType)
        {
            throw new NotImplementedException();
        }

        public void SendDiagnosticMessage(string message)
        {
        }

        public void SendDiagnosticMessage(string format, object? arg0)
        {
        }

        public void SendDiagnosticMessage(string format, object? arg0, object? arg1)
        {
        }

        public void SendDiagnosticMessage(string format, object? arg0, object? arg1, object? arg2)
        {
        }

        public void SendDiagnosticMessage(string format, params object?[] args)
        {
        }
    }
}