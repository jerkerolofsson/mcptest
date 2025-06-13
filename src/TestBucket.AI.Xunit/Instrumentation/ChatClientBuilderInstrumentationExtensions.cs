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
    /// <summary>
    /// Provides extension methods for <see cref="ChatClientBuilder"/> to add instrumentation and service-providing chat clients.
    /// </summary>
    public static class ChatClientBuilderInstrumentationExtensions
    {
        /// <summary>
        /// Adds an <see cref="InstrumentationChatClient"/> to the <see cref="ChatClientBuilder"/> pipeline.
        /// This enables instrumentation of chat client interactions for testing or monitoring purposes.
        /// </summary>
        /// <param name="builder">The chat client builder to extend.</param>
        /// <returns>The same <see cref="ChatClientBuilder"/> instance for chaining.</returns>
        public static ChatClientBuilder UseInstrumentationChatClient(this ChatClientBuilder builder)
        {
            return builder.Use(delegate (IChatClient innerClient, IServiceProvider services)
            {
                InstrumentationChatClient instrumentation = new InstrumentationChatClient(innerClient);
                return instrumentation;
            });
        }

        /// <summary>
        /// Adds a <see cref="ServiceProvidingClient"/> to the <see cref="ChatClientBuilder"/> pipeline.
        /// This allows the IChatClient to access services from the dependency injection container.
        /// </summary>
        /// <param name="builder">The chat client builder to extend.</param>
        /// <returns>The same <see cref="ChatClientBuilder"/> instance for chaining.</returns>
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