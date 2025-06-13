using System.Diagnostics;

using TestBucket.AI.Xunit.Instrumentation;

namespace TestBucket.AI.Xunit.Benchmarking;

/// <summary>
/// Builds and updates a <see cref="BenchmarkResult"/> during the execution of a benchmark,
/// tracking iteration state, exceptions, and instrumentation results.
/// </summary>
internal class BenchmarkResultBuilder(BenchmarkResult Result)
{
    private long _startTimestamp = 0;

    /// <summary>
    /// Marks the start of a benchmark iteration, recording the timestamp and incrementing the started count.
    /// </summary>
    internal void OnStart()
    {
        _startTimestamp = Stopwatch.GetTimestamp();
        Result.IterationsStarted++;
    }

    /// <summary>
    /// Records an exception that occurred during the benchmark execution,
    /// marks the iteration as failed, and adds the exception to the result.
    /// </summary>
    /// <param name="ex">The exception that occurred.</param>
    internal void OnException(Exception ex)
    {
        OnIterationCompleted();
        Result.IterationsFailed++;
        Result.AddException(ex);
    }

    /// <summary>
    /// Records an exception that occurred during user verification of a result,
    /// marks the iteration as failed, and adds the exception to the result.
    /// </summary>
    /// <param name="ex">The exception that occurred during verification.</param>
    internal void OnVerificationException(Exception ex)
    {
        OnIterationCompleted();
        Result.IterationsFailed++;
        Result.AddException(ex);
    }

    /// <summary>
    /// Marks the completion of a benchmark iteration and accumulates the elapsed duration.
    /// </summary>
    private void OnIterationCompleted()
    {
        var elapsed = Stopwatch.GetElapsedTime(_startTimestamp);
        Result.AccumulatedDuration += elapsed;
    }

    /// <summary>
    /// Marks the iteration as passed after successful verification and accumulates the elapsed duration.
    /// </summary>
    internal void OnSuccess()
    {
        OnIterationCompleted();
        Result.IterationsPassed++;
    }

    /// <summary>
    /// Adds an instrumentation test result to the benchmark result.
    /// </summary>
    /// <param name="result">The instrumentation test result to add.</param>
    internal void OnInstrumentationTestResult(InstrumentationTestResult result)
    {
        Result.AddInstrumentationTestResult(result);
    }
}