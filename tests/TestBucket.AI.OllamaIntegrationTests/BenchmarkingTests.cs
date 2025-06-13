using TestBucket.AI.Xunit;
using TestBucket.AI.Xunit.Benchmarking;
using TestBucket.AI.Xunit.Ollama;

namespace TestBucket.AI.OllamaIntegrationTests;

/// <summary>
/// Contains integration tests for benchmarking different Ollama models using instrumented chat clients.
/// </summary>
/// <remarks>
/// This class benchmarks multiple models by running a simple addition task and reports the pass rate and exceptions for each model.
/// </remarks>
[EnrichedTest]
[IntegrationTest]
public class BenchmarkingTests(OllamaFixture Ollama)
{
    /// <summary>
    /// Benchmarks two different models by running a simple addition task and reports the results.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    [Fact]
    public async Task Benchmark_TwoModels()
    {
        foreach (string model in new string[] { "llama3.1:8b", "qwen3:8b" })
        {
            var client = await CalculatorUtils.CreateInstrumentedChatClientAsync(Ollama, model);
            var benchmarkResult = await client.BencharkAsync("Add 3 and 6", iterations: 2, (iterationResult) =>
            {
                iterationResult.ShouldBeSuccess();
                iterationResult.ContainsFunctionCall("Add");
            });

            // Write summary
            TestContext.Current.TestOutputHelper?.WriteLine($"Model: {model}, Passrate={benchmarkResult.Passrate}");

            // Write exceptions
            foreach (var exception in benchmarkResult.Exceptions)
            {
                TestContext.Current.TestOutputHelper?.WriteLine(exception.ToString());
            }
        }
    }
}