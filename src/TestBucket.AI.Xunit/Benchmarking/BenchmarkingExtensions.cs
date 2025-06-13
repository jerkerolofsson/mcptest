using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.Benchmarking;

/// <summary>
/// Provides extension methods for benchmarking chat client interactions.
/// </summary>
public static class BenchmarkingExtensions
{
    /// <summary>
    /// Benchmarks a chat client using a single user prompt for a specified number of iterations.
    /// </summary>
    /// <param name="chatClient">The chat client to benchmark.</param>
    /// <param name="userPrompt">The user prompt to send.</param>
    /// <param name="iterations">The number of iterations to run. Default is 1.</param>
    /// <param name="verifier">An optional action to verify each <see cref="InstrumentationTestResult"/>.</param>
    /// <returns>A <see cref="BenchmarkResult"/> containing the results of the benchmark.</returns>
    public static async ValueTask<BenchmarkResult> BencharkAsync(this IChatClient chatClient, string userPrompt, int iterations = 1, Action<InstrumentationTestResult>? verifier = null)
    {
        ChatMessage[] messages = [new ChatMessage(ChatRole.User, userPrompt)];
        return await BencharkAsync(chatClient, messages, iterations, verifier);
    }

    /// <summary>
    /// Benchmarks a chat client using a sequence of chat messages for a specified number of iterations.
    /// </summary>
    /// <param name="chatClient">The chat client to benchmark.</param>
    /// <param name="chatMessages">The chat messages to send.</param>
    /// <param name="iterations">The number of iterations to run. Default is 1.</param>
    /// <param name="verifier">An optional action to verify each <see cref="InstrumentationTestResult"/>.</param>
    /// <returns>A <see cref="BenchmarkResult"/> containing the results of the benchmark.</returns>
    public static async ValueTask<BenchmarkResult> BencharkAsync(this IChatClient chatClient, IEnumerable<ChatMessage> chatMessages, int iterations = 1, Action<InstrumentationTestResult>? verifier = null)
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        var parameters = new BenchmarkParameters { ChatMessages = chatMessages.ToArray(), Iterations = iterations };
        return await BenchmarkAsync(chatClient, chatMessages, verifier, parameters, cancellationToken);
    }

    /// <summary>
    /// Executes the benchmark for the specified chat client and messages, using the provided parameters and verifier.
    /// </summary>
    /// <param name="chatClient">The chat client to benchmark.</param>
    /// <param name="chatMessages">The chat messages to send.</param>
    /// <param name="verifier">An optional action to verify each <see cref="InstrumentationTestResult"/>.</param>
    /// <param name="parameters">The benchmark parameters.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    /// <returns>A <see cref="BenchmarkResult"/> containing the results of the benchmark.</returns>
    public static async Task<BenchmarkResult> BenchmarkAsync(
        IChatClient chatClient,
        IEnumerable<ChatMessage> chatMessages,
        Action<InstrumentationTestResult>? verifier,
        BenchmarkParameters parameters,
        CancellationToken cancellationToken)
    {
        var benchmarkResult = new BenchmarkResult(parameters);

        for (int i = 0; i < benchmarkResult.Parameters.Iterations; i++)
        {
            await RunOneIterationAsync(chatClient, chatMessages, verifier, parameters, benchmarkResult, cancellationToken);
        }

        TestContext.Current.AttachBenchmarkResult(benchmarkResult);

        return benchmarkResult;
    }

    /// <summary>
    /// Runs a single iteration of the benchmark, collecting results and handling exceptions.
    /// </summary>
    /// <param name="chatClient">The chat client to use.</param>
    /// <param name="chatMessages">The chat messages to send.</param>
    /// <param name="verifier">An optional action to verify the <see cref="InstrumentationTestResult"/>.</param>
    /// <param name="parameters">The benchmark parameters.</param>
    /// <param name="benchmarkResult">The benchmark result to update.</param>
    /// <param name="cancellationToken">A cancellation token to observe while waiting for the task to complete.</param>
    private static async Task RunOneIterationAsync(
        IChatClient chatClient,
        IEnumerable<ChatMessage> chatMessages,
        Action<InstrumentationTestResult>? verifier,
        BenchmarkParameters parameters,
        BenchmarkResult benchmarkResult,
        CancellationToken cancellationToken)
    {
        var resultBuilder = new BenchmarkResultBuilder(benchmarkResult);
        resultBuilder.OnStart();
        try
        {
            var result = await chatClient.TestGetResponseAsync(chatMessages, parameters.ConfigureOptions, addToReport: false, cancellationToken);

            resultBuilder.OnInstrumentationTestResult(result);

            try
            {
                // Callback to the test to let them verify the result for each iteration
                if (verifier is not null)
                {
                    verifier(result);
                }
                resultBuilder.OnSuccess();
            }
            catch (Exception ex)
            {
                resultBuilder.OnVerificationException(ex);
            }
        }
        catch (Exception ex)
        {
            resultBuilder.OnException(ex);
        }
    }
}