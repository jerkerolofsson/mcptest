using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.Ollama
{
    /// <summary>
    /// Provides a test fixture for managing an Ollama container and creating chat clients for integration testing.
    /// </summary>
    public class OllamaFixture : IAsyncLifetime
    {
        private OllamaContainer? _ollamaContainer;
        private OllamaApiClient? _ollamaClient;

        /// <summary>
        /// Gets the base URL for the Ollama API running in the container.
        /// </summary>
        public string OllamaUrl => $"http://{_ollamaContainer?.Hostname}:{_ollamaContainer?.GetMappedPublicPort(11434)}";

        /// <summary>
        /// Initializes the Ollama container and prepares it for use in tests.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public async ValueTask InitializeAsync()
        {
            _ollamaContainer = new OllamaBuilder()
                .WithVolumeMount("TestBucket.AI.Xunit.Ollama.OllamaFixture", "/root/.ollama")
                .WithPortBinding(11434, true)
                .Build();
            await _ollamaContainer.StartAsync(TestContext.Current.CancellationToken);
        }

        /// <summary>
        /// Creates a new chat client for the specified model, optionally configuring services and tools.
        /// </summary>
        /// <param name="modelName">The name of the model to use.</param>
        /// <param name="configureServices">An optional action to configure additional services.</param>
        /// <param name="configureTools">An optional action to configure additional tools.</param>
        /// <returns>
        /// A <see cref="ValueTask{IChatClient}"/> representing the asynchronous operation, with the created chat client as the result.
        /// </returns>
        public async ValueTask<IChatClient> CreateChatClientAsync(string modelName,
            Action<ServiceCollection>? configureServices = null,
            Action<IToolConfigurator>? configureTools = null)
        {
            _ollamaClient ??= new OllamaApiClient(OllamaUrl, modelName);
            await PullModelAsync(modelName);

            var client = InstrumentationChatClientFactory.Create(_ollamaClient, configureServices, configureTools);

            // Set the mode name for logging
            var result = client.GetRequiredService<InstrumentationTestResult>();
            result.ModelName = modelName;
            result.ProviderName = "ollama";
            result.ProviderVersion = await _ollamaClient.GetVersionAsync(TestContext.Current.CancellationToken);

            return client;
        }

        /// <summary>
        /// Pulls the specified model from the Ollama API.
        /// </summary>
        /// <param name="modelName">The name of the model to pull.</param>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        private async ValueTask PullModelAsync(string modelName)
        {
            await foreach (var _ in _ollamaClient!.PullModelAsync(modelName, TestContext.Current.CancellationToken))
            {

            }
        }

        /// <summary>
        /// Disposes the Ollama container and client resources asynchronously.
        /// </summary>
        /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
        public async ValueTask DisposeAsync()
        {
            if (_ollamaContainer is not null)
            {
                await _ollamaContainer.DisposeAsync();
            }
            if (_ollamaClient is not null)
            {
                _ollamaClient.Dispose();
            }
        }
    }
}