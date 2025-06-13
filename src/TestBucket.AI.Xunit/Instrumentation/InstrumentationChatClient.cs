using System.Diagnostics;
using System.Runtime.CompilerServices;

using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;

namespace TestBucket.AI.Xunit.Instrumentation
{
    /// <summary>
    /// A chat client that instruments and records activities, requests, and responses for testing purposes.
    /// </summary>
    internal class InstrumentationChatClient : DelegatingChatClient
    {
        private readonly ActivityListener _listener;
        private readonly List<Activity> _activities = [];
        private readonly InstrumentationTestResult _result;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstrumentationChatClient"/> class.
        /// </summary>
        /// <param name="innerClient">The inner <see cref="IChatClient"/> to delegate calls to.</param>
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

        /// <summary>
        /// Releases the unmanaged resources used by the <see cref="InstrumentationChatClient"/> and optionally releases the managed resources.
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _listener.Dispose();
        }

        /// <summary>
        /// Gets a streaming chat response asynchronously and inspects each update for instrumentation.
        /// </summary>
        /// <param name="messages">The chat messages to send.</param>
        /// <param name="options">Optional chat options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>An async enumerable of <see cref="ChatResponseUpdate"/>.</returns>
        public override async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null,
            [EnumeratorCancellation]
            CancellationToken cancellationToken = default)
        {
            LogRequestMessagesToResult(messages);

            await foreach (var response in InnerClient.GetStreamingResponseAsync(messages, options, cancellationToken))
            {
                cancellationToken.ThrowIfCancellationRequested();
                InspectAndRecordResponseUpdate(response);
                yield return response;
            }
        }

        /// <summary>
        /// Inspects a <see cref="ChatResponseUpdate"/> and records relevant content to the instrumentation result.
        /// </summary>
        /// <param name="response">The chat response update to inspect.</param>
        private void InspectAndRecordResponseUpdate(ChatResponseUpdate response)
        {
            foreach (var content in response.Contents)
            {
                RecordContent(content);
            }
        }

        /// <summary>
        /// Records a single content item to the instrumentation result.
        /// </summary>
        /// <param name="content">The content to record.</param>
        private void RecordContent(AIContent content)
        {
            switch (content)
            {
                case FunctionCallContent functionCallContent:
                    _result.AddFunctionCall(functionCallContent);
                    break;
                case FunctionResultContent functionResultContent:
                    RecordFunctionResultContent(functionResultContent);
                    break;
                case TextContent textContent:
                    _result.AddTextContent(textContent);
                    break;
                default:
                    _result.AddOtherContent(content);
                    break;
            }
        }

        /// <summary>
        /// Records a function result content, including any exception.
        /// </summary>
        /// <param name="functionResultContent">The function result content to record.</param>
        private void RecordFunctionResultContent(FunctionResultContent functionResultContent)
        {
            if (functionResultContent.Exception is not null)
            {
                _result.AddFunctionCallException(functionResultContent.Exception);
            }
        }

        /// <summary>
        /// Inspects a <see cref="ChatResponse"/> and records relevant updates to the instrumentation result.
        /// </summary>
        /// <param name="response">The chat response to inspect.</param>
        private void InspectAndRecordResponse(ChatResponse response)
        {
            foreach (var update in response.ToChatResponseUpdates())
            {
                InspectAndRecordResponseUpdate(update);
            }
        }

        /// <summary>
        /// Gets a chat response asynchronously, inspects it, and records instrumentation data.
        /// </summary>
        /// <param name="messages">The chat messages to send.</param>
        /// <param name="options">Optional chat options.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>A <see cref="Task{ChatResponse}"/> representing the asynchronous operation.</returns>
        public override async Task<ChatResponse> GetResponseAsync(IEnumerable<ChatMessage> messages, ChatOptions? options = null, CancellationToken cancellationToken = default)
        {
            _activities.Clear();

            LogRequestMessagesToResult(messages);

            var response = await InnerClient.GetResponseAsync(messages, options, cancellationToken);
            InspectAndRecordResponse(response);
            LogUsageToResult(response);
            LogAllActivitiesToResult();

            return response;
        }

        /// <summary>
        /// Logs the request messages to the instrumentation result.
        /// </summary>
        /// <param name="messages">The chat messages to log.</param>
        private void LogRequestMessagesToResult(IEnumerable<ChatMessage> messages)
        {
            _result.AddRequestMessages(messages);
        }

        /// <summary>
        /// Logs all collected activities to the instrumentation result.
        /// </summary>
        private void LogAllActivitiesToResult()
        {
            foreach (var activity in _activities)
            {
                LogActivityToResult(activity);
            }
        }

        /// <summary>
        /// Logs an activity to the instrumentation result.
        /// </summary>
        /// <param name="activity">The activity to log.</param>
        private void LogActivityToResult(Activity activity)
        {
            _result.AddActivity(activity);
        }

        /// <summary>
        /// Logs usage statistics from a chat response to the instrumentation result.
        /// </summary>
        /// <param name="response">The chat response containing usage data.</param>
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