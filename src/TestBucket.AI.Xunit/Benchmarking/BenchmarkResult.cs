using TestBucket.AI.Xunit.Instrumentation;

namespace TestBucket.AI.Xunit.Benchmarking;

/// <summary>
/// Represents the result of a benchmark run, including statistics, exceptions, and instrumentation results.
/// </summary>
public class BenchmarkResult
{
    private readonly List<Exception> _exceptions = [];

    private readonly List<InstrumentationTestResult> _instrumentationTestResults = [];

    /// <summary>
    /// Gets the list of exceptions that occurred during the benchmark run.
    /// </summary>
    public IReadOnlyList<Exception> Exceptions => _exceptions.AsReadOnly();

    /// <summary>
    /// Gets the list of instrumentation test results collected during the benchmark run.
    /// </summary>
    public IReadOnlyList<InstrumentationTestResult> InstrumentationTestResults => _instrumentationTestResults.AsReadOnly();

    /// <summary>
    /// Gets the parameters used for this benchmark run.
    /// </summary>
    public BenchmarkParameters Parameters { get; }

    /// <summary>
    /// Gets or sets the number of iterations that passed.
    /// </summary>
    public int IterationsPassed { get; internal set; }

    /// <summary>
    /// Gets or sets the number of iterations that started.
    /// </summary>
    public int IterationsStarted { get; internal set; }

    /// <summary>
    /// Gets or sets the number of iterations that failed.
    /// </summary>
    public int IterationsFailed { get; internal set; }

    /// <summary>
    /// Gets a value indicating whether the benchmark passed (pass rate is 100%).
    /// </summary>
    public bool IsPassed => Passrate >= 1.0;

    /// <summary>
    /// Gets or sets the total accumulated duration of all benchmark iterations.
    /// </summary>
    public TimeSpan AccumulatedDuration { get; internal set; }

    /// <summary>
    /// Gets the pass rate as a value between 0.0 and 1.0.
    /// </summary>
    public double Passrate
    {
        get
        {
            if (IterationsStarted > 0)
            {
                return IterationsPassed / (double)IterationsStarted;
            }
            return 0.0;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BenchmarkResult"/> class with the specified parameters.
    /// </summary>
    /// <param name="parameters">The parameters used for the benchmark run.</param>
    public BenchmarkResult(BenchmarkParameters parameters)
    {
        Parameters = parameters;
    }

    /// <summary>
    /// Adds an exception to the list of exceptions for this benchmark result.
    /// </summary>
    /// <param name="ex">The exception to add.</param>
    internal void AddException(Exception ex)
    {
        _exceptions.Add(ex);
    }

    /// <summary>
    /// Adds an instrumentation test result to the list for this benchmark result.
    /// </summary>
    /// <param name="result">The instrumentation test result to add.</param>
    internal void AddInstrumentationTestResult(InstrumentationTestResult result)
    {
        _instrumentationTestResults.Add(result);
    }
}