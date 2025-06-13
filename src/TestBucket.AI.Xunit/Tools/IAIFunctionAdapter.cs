using Microsoft.Extensions.AI;

namespace TestBucket.AI.Xunit.Tools
{
    /// <summary>
    /// Defines an adapter for an AI function, exposing its metadata and functionality.
    /// </summary>
    public interface IAIFunctionAdapter
    {
        /// <summary>
        /// Gets the underlying <see cref="AIFunction"/> instance represented by this adapter.
        /// </summary>
        AIFunction AIFunction { get; }

        /// <summary>
        /// Gets the name of the AI function.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the AI function.
        /// </summary>
        string Description { get; }
    }
}