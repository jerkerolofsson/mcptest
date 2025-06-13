using System.Diagnostics;
using Microsoft.Extensions.AI;

namespace TestBucket.AI.Xunit.Instrumentation
{
    public record class InstrumentationTestResult
    {
        private readonly List<Activity> _activities = [];
        private readonly List<Exception> _exceptions = [];
        private readonly List<FunctionCallContent> _functionCalls = [];
        private readonly List<AIContent> _otherContent = [];
        private readonly List<TextContent> _textContent = [];
        private readonly List<ChatMessage> _requestMessages = [];

        /// <summary>
        /// Provide name (e.g. ollama)
        /// </summary>
        public string? ProviderName { get; set; }

        /// <summary>
        /// Version
        /// </summary>
        public string ProviderVersion { get; set; }

        /// <summary>
        /// Name of model used
        /// </summary>
        public string? ModelName { get; set; }

        public long? OutputTokenCount { get; set; }
        public long? InputTokenCount { get; set; }
        public long? TotalTokenCount { get; set; }

        public IReadOnlyList<Activity> Activities => _activities.AsReadOnly();
        public IReadOnlyList<FunctionCallContent> FunctionCalls => _functionCalls.AsReadOnly();
        public IReadOnlyList<Exception> FunctionCallExceptions => _exceptions.AsReadOnly();
        public IReadOnlyList<ChatMessage> RequestMessages => _requestMessages.AsReadOnly();

        
        internal void AddActivity(Activity activity)
        {
            if (activity is not null)
            {
                _activities.Add(activity);
            }
        }

        internal void AddFunctionCall(FunctionCallContent functionCall)
        {
            if (functionCall is not null)
            {
                _functionCalls.Add(functionCall);
            }
        }

        internal void AddFunctionCallException(Exception exception)
        {
            if (exception is not null)
            {
                _exceptions.Add(exception);
            }
        }

        internal void AddOtherContent(AIContent content)
        {
            if(content is not null)
            {
                _otherContent.Add(content);
            }
        }

        internal void AddRequestMessages(IEnumerable<ChatMessage> messages)
        {
            _requestMessages.AddRange(messages);
        }

        internal void AddTextContent(TextContent textContent)
        {
            if(textContent is not null)
            {
                _textContent.Add(textContent);
            }
        }
    }
}
