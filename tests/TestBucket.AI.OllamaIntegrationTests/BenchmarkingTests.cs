using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Benchmarking;
using TestBucket.AI.Xunit.Ollama;
using TestBucket.AI.Xunit;

namespace TestBucket.AI.OllamaIntegrationTests;

[EnrichedTest]
[IntegrationTest]
public class BenchmarkingTests(OllamaFixture Ollama)
{

    /// <summary>
    /// Benchmarks two different models
    /// </summary>
    /// <returns></returns>
    [Fact]
    public async Task Benchmark_TwoModels()
    {
        foreach (string model in new string[] { "llama3.1:8b", "qwen3:8b" })
        {
            var client = await CalculatorUtils.CreateInstrumentedChatClientAsync(Ollama, model);
            var benchmarkResult = await client.BencharkAsync("Add 3 and 6", iterations:2, (iterationResult) =>
            {
                iterationResult.ShouldBeSuccess();
                iterationResult.ContainsFunctionCall("Add");
            });

            // Write summary
            TestContext.Current.TestOutputHelper?.WriteLine($"Model: {model}, Passrate={benchmarkResult.Passrate}");

            // Write exceptions
            foreach(var exception in benchmarkResult.Exceptions)
            {
                TestContext.Current.TestOutputHelper?.WriteLine(exception.ToString());
            }
        }
    }
}
