using Microsoft.Extensions.AI;

namespace TestBucket.AI.Xunit.Benchmarking;

/// <summary>
/// Represents the parameters used to configure a chat benchmarking run.
/// </summary>
public record class BenchmarkParameters
{
    /// <summary>
    /// Optional delegate to configure <see cref="ChatOptions"/> before each benchmark run.
    /// </summary>
    public Action<ChatOptions>? ConfigureOptions { get; set; }

    /// <summary>
    /// The sequence of chat messages to use as input for the benchmark.
    /// </summary>
    public required ChatMessage[] ChatMessages { get; set; }

    /// <summary>
    /// The number of iterations to execute the benchmark. Defaults to 1.
    /// </summary>
    public int Iterations { get; set; } = 1;
}