using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace TestBucket.AI.Xunit.Instrumentation
{
    internal class InstrumentationChatClient : DelegatingChatClient
    {
        private readonly ActivityListener _listener;
        private readonly List<Activity> _activities = [];
        private readonly InstrumentationTestResult _result;

        internal InstrumentationChatClient(IChatClient innerClient) : base(innerClient)
        {
            _result = innerClient.GetRequiredService<InstrumentationTestResult>();

            var activities = new List<Activity>();

            _listener = new ActivityListener
            {
                ShouldListenTo = source => source.Name == InstrumentationConstants.ActivitySourceName,
                Sample = (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
                ActivityStarted = activity => _activities.Add(activity),
                ActivityStopped = activity => { /* Optionally handle stop */ }
            };
            ActivitySource.AddActivityListener(_listener);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _listener.Dispose();
        }

        public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            await foreach(var response in InnerClient.GetStreamingResponseAsync(messages, options, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                Inspect(response);
                yield return response;
            }
        }

        private void Inspect(ChatResponseUpdate response)
        {
            foreach(var content in response.Contents)
            {
                if (content is FunctionCallContent functionCallContent)
                {
                    _result.AddFunctionCall(functionCallContent);
                }
                else if (content is FunctionResultContent functionResultContent)
                {
                    if (functionResultContent.Exception is not null)
                    {
                        _result.AddFunctionCallException(functionResultContent.Exception);
                    }
                }
                else if (content is TextContent textContent)
                {
                    _result.AddTextContent(textContent);
                }
                else 
                {
                    _result.AddOtherContent(content);
                }
            }
        }

        private void Inspect(ChatResponse response)
        {
            foreach(var update in response.ToChatResponseUpdates())
            {
                // Inspect the content here if needed
                Inspect(update);
            }
        }
        public override async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            _activities.Clear();

            var response = await InnerClient.GetResponseAsync(messages, options, cancellationToken);
            Inspect(response);
            LogUsageToResult(response);

            foreach (var activity in _activities)
            {
                LogActivityToResult(activity);
            }

            return response;
        }

        private void LogActivityToResult(Activity activity)
        {
            _result.AddActivity(activity);
        }

        private void LogUsageToResult(ChatResponse response)
        {
            if (response.Usage?.InputTokenCount is not null)
            {
                _result.InputTokenCount ??= 0;
                _result.InputTokenCount += response.Usage.InputTokenCount.Value;
            }
            if (response.Usage?.OutputTokenCount is not null)
            {
                _result.OutputTokenCount ??= 0;
                _result.OutputTokenCount += response.Usage.OutputTokenCount.Value;
            }
            if (response.Usage?.TotalTokenCount is not null)
            {
                _result.TotalTokenCount ??= 0;
                _result.TotalTokenCount += response.Usage.TotalTokenCount.Value;
            }
        }
    }
}
