using System;
using System.Collections.Generic;

using TestBucket.AI.Xunit.Benchmarking;
using TestBucket.AI.Xunit.Instrumentation;

using Xunit;

namespace TestBucket.AI.Xunit.UnitTests.Benchmarking;

/// <summary>
/// Contains unit tests for the <see cref="BenchmarkResult"/> class.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Benchmarking")]
[Component("Benchmarking")]
public class BenchmarkResultTests
{
    /// <summary>
    /// Verifies that the constructor sets the <see cref="BenchmarkResult.Parameters"/> property.
    /// </summary>
    [Fact]
    public void Constructor_SetsParameters()
    {
        var parameters = new BenchmarkParameters { ChatMessages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>() };
        var result = new BenchmarkResult(parameters);

        Assert.Equal(parameters, result.Parameters);
    }

    /// <summary>
    /// Verifies that <see cref="BenchmarkResult.AddException(Exception)"/> adds exceptions to the <see cref="BenchmarkResult.Exceptions"/> list.
    /// </summary>
    [Fact]
    public void AddException_AddsToExceptionsList()
    {
        var result = new BenchmarkResult(new BenchmarkParameters { ChatMessages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>() });
        var ex = new InvalidOperationException();

        result.AddException(ex);

        Assert.Contains(ex, result.Exceptions);
    }

    /// <summary>
    /// Verifies that <see cref="BenchmarkResult.AddInstrumentationTestResult(InstrumentationTestResult)"/> adds results to the <see cref="BenchmarkResult.InstrumentationTestResults"/> list.
    /// </summary>
    [Fact]
    public void AddInstrumentationTestResult_AddsToList()
    {
        var result = new BenchmarkResult(new BenchmarkParameters { ChatMessages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>() });
        var testResult = new InstrumentationTestResult();

        result.AddInstrumentationTestResult(testResult);

        Assert.Contains(testResult, result.InstrumentationTestResults);
    }

    /// <summary>
    /// Verifies that <see cref="BenchmarkResult.Passrate"/> calculates correctly.
    /// </summary>
    [Theory]
    [InlineData(10, 10, 1.0)]
    [InlineData(5, 10, 0.5)]
    [InlineData(0, 10, 0.0)]
    [InlineData(0, 0, 0.0)]
    public void Passrate_CalculatesCorrectly(int passed, int started, double expected)
    {
        var result = new BenchmarkResult(new BenchmarkParameters { ChatMessages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>() })
        {
            IterationsPassed = passed,
            IterationsStarted = started
        };

        Assert.Equal(expected, result.Passrate, 3);
    }

    /// <summary>
    /// Verifies that <see cref="BenchmarkResult.IsPassed"/> returns true only when passrate is 1.0.
    /// </summary>
    [Theory]
    [InlineData(10, 10, true)]
    [InlineData(9, 10, false)]
    [InlineData(0, 0, false)]
    public void IsPassed_ReturnsTrueOnlyWhenPassrateIsOne(int passed, int started, bool expected)
    {
        var result = new BenchmarkResult(new BenchmarkParameters { ChatMessages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>() })
        {
            IterationsPassed = passed,
            IterationsStarted = started
        };

        Assert.Equal(expected, result.IsPassed);
    }

    /// <summary>
    /// Verifies that <see cref="BenchmarkResult.AccumulatedDuration"/> can be set and retrieved.
    /// </summary>
    [Fact]
    public void AccumulatedDuration_CanBeSetAndRetrieved()
    {
        var result = new BenchmarkResult(new BenchmarkParameters { ChatMessages = Array.Empty<Microsoft.Extensions.AI.ChatMessage>() })
        {
            AccumulatedDuration = TimeSpan.FromSeconds(42)
        };

        Assert.Equal(TimeSpan.FromSeconds(42), result.AccumulatedDuration);
    }
}