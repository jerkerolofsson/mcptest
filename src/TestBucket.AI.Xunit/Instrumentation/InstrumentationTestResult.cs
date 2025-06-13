using System.Diagnostics;

using Microsoft.Extensions.AI;

namespace TestBucket.AI.Xunit.Instrumentation
{
    /// <summary>
    /// Represents the result of an instrumented test, capturing activities, function calls, exceptions, and related AI content.
    /// </summary>
    public record class InstrumentationTestResult
    {
        private readonly List<Activity> _activities = [];
        private readonly List<Exception> _exceptions = [];
        private readonly List<FunctionCallContent> _functionCalls = [];
        private readonly List<AIContent> _otherContent = [];
        private readonly List<TextContent> _textContent = [];
        private readonly List<ChatMessage> _requestMessages = [];

        /// <summary>
        /// Gets or sets the provider name (e.g. "ollama").
        /// </summary>
        public string? ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider version.
        /// </summary>
        public string? ProviderVersion { get; set; }

        /// <summary>
        /// Gets or sets the name of the model used.
        /// </summary>
        public string? ModelName { get; set; }

        /// <summary>
        /// Gets or sets the number of output tokens.
        /// </summary>
        public long? OutputTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the number of input tokens.
        /// </summary>
        public long? InputTokenCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of tokens.
        /// </summary>
        public long? TotalTokenCount { get; set; }

        /// <summary>
        /// Gets the list of activities recorded during the test.
        /// </summary>
        public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();

        /// <summary>
        /// Gets the list of function calls made during the test.
        /// </summary>
        public IReadOnlyList<FunctionCallContent> FunctionCalls => _functionCalls.AsReadOnly();

        /// <summary>
        /// Gets the list of exceptions thrown during function calls.
        /// </summary>
        public IReadOnlyList<Exception> FunctionCallExceptions => _exceptions.AsReadOnly();

        /// <summary>
        /// Gets the list of request messages sent during the test.
        /// </summary>
        public IReadOnlyList<ChatMessage> RequestMessages => _requestMessages.AsReadOnly();

        /// <summary>
        /// Adds an <see cref="Activity"/> to the result.
        /// </summary>
        /// <param name="activity">The activity to add.</param>
        internal void AddActivity(Activity activity)
        {
            if (activity is not null)
            {
                _activities.Add(activity);
            }
        }

        /// <summary>
        /// Adds a <see cref="FunctionCallContent"/> to the result.
        /// </summary>
        /// <param name="functionCall">The function call to add.</param>
        internal void AddFunctionCall(FunctionCallContent functionCall)
        {
            if (functionCall is not null)
            {
                _functionCalls.Add(functionCall);
            }
        }

        /// <summary>
        /// Adds an <see cref="Exception"/> thrown during a function call to the result.
        /// </summary>
        /// <param name="exception">The exception to add.</param>
        internal void AddFunctionCallException(Exception exception)
        {
            if (exception is not null)
            {
                _exceptions.Add(exception);
            }
        }

        /// <summary>
        /// Adds additional AI content to the result.
        /// </summary>
        /// <param name="content">The AI content to add.</param>
        internal void AddOtherContent(AIContent content)
        {
            if (content is not null)
            {
                _otherContent.Add(content);
            }
        }

        /// <summary>
        /// Adds a collection of request messages to the result.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        internal void AddRequestMessages(IEnumerable<ChatMessage> messages)
        {
            _requestMessages.AddRange(messages);
        }

        /// <summary>
        /// Adds text content to the result.
        /// </summary>
        /// <param name="textContent">The text content to add.</param>
        internal void AddTextContent(TextContent textContent)
        {
            if (textContent is not null)
            {
                _textContent.Add(textContent);
            }
        }

        internal void Clear()
        {
            _functionCalls.Clear();
            _requestMessages.Clear();
            _exceptions.Clear();
            _activities.Clear();
            OutputTokenCount = 0;
            InputTokenCount = 0;
            TotalTokenCount = 0;
        }
    }
}