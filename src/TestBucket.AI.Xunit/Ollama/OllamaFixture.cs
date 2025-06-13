using System.Diagnostics;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit.Instrumentation;
using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.Ollama
{
    public class OllamaFixture : IAsyncLifetime
    {
        private OllamaContainer? _ollamaContainer;
        private OllamaApiClient? _ollamaClient;

        public string OllamaUrl => $"http://localhost:{_ollamaContainer?.GetMappedPublicPort(11434)}";

        public async ValueTask InitializeAsync()
        {
            _ollamaContainer = new OllamaBuilder()
                .WithVolumeMount("TestBucket.AI.Xunit.Ollama.OllamaFixture", "/root/.ollama")
                .WithPortBinding(11434, true)
                .Build();
            await _ollamaContainer.StartAsync(TestContext.Current.CancellationToken);
        }

        public async ValueTask<IChatClient> CreateChatClientAsync(string modelName, 
            Action<ServiceCollection>? configureServices = null,
            Action<IToolConfigurator>? configureTools = null)
        {
            _ollamaClient ??= new OllamaApiClient(OllamaUrl, modelName);
            await PullModelAsync(modelName);

            return InstrumentationChatClientFactory.Create(_ollamaClient, configureServices, configureTools);
        }

        private async ValueTask PullModelAsync(string modelName)
        {
            await foreach (var _ in _ollamaClient!.PullModelAsync(modelName, TestContext.Current.CancellationToken))
            {

            }
        }

        public async ValueTask DisposeAsync()
        {
            if (_ollamaContainer is not null)
            {
                await _ollamaContainer.DisposeAsync();
            }
            if(_ollamaClient is not null)
            {
                _ollamaClient.Dispose();
            }
        }
    }
}
