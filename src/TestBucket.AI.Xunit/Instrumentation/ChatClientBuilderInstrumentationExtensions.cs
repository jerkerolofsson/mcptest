using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestBucket.AI.Xunit.Instrumentation
{
    public static class ChatClientBuilderInstrumentationExtensions
    {
        public static ChatClientBuilder UseInstrumentationChatClient(this ChatClientBuilder builder)
        {
            return builder.Use(delegate (IChatClient innerClient, IServiceProvider services)
            {
                InstrumentationChatClient instrumentation = new InstrumentationChatClient(innerClient);
                return instrumentation;
            });
        }
        public static ChatClientBuilder UseServiceProvidingChatClient(this ChatClientBuilder builder)
        {
            return builder.Use(delegate (IChatClient innerClient, IServiceProvider services)
            {
                var client = new ServiceProvidingClient(innerClient, services);
                return client;
            });
        }
    }
}
