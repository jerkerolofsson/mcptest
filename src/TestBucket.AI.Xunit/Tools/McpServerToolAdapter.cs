using Microsoft.Extensions.AI;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Reflection;

namespace TestBucket.AI.Xunit.Tools;

/// <summary>
/// Adapter that allows calling ModelContextProtocol.Server tools directly using Microsoft.Extensions.AI.IChatClient, reading
/// tool-name from McpServerToolAttribute
/// </summary>
public class McpServerToolAdapter : IAIFunctionAdapter
{
    private readonly McpServerToolAttribute? _mcpServerToolAttribute;
    private readonly DescriptionAttribute? _descriptionAttribute;

    private readonly Dictionary<string, object?> _additionalProperties = [];
    private readonly Delegate _method;

    /// <summary>
    /// Gets the name of the tool.
    /// </summary>
    public string Name => _mcpServerToolAttribute?.Name ?? "";

    /// <summary>
    /// Gets a description of the tool, suitable for use in describing the purpose to
    //     a model.
    /// </summary>
    public string Description => _descriptionAttribute?.Description ?? "";

    /// <summary>
    /// Gets any additional properties associated with the tool.
    /// </summary>
    public IReadOnlyDictionary<string, object?> AdditionalProperties => _additionalProperties;

    /// <summary>
    /// The AI Function created
    /// </summary>
    public AIFunction AIFunction { get; }

    public McpServerToolAdapter(Delegate method)
    {
        var methodInfo = method.Method;
        var serverToolAttribute = methodInfo.GetCustomAttribute<McpServerToolAttribute>();
        if(serverToolAttribute is null)
        {
            //throw new ArgumentException("Expected method to have a McpServerToolAttribute");
        }
        _mcpServerToolAttribute = serverToolAttribute;
        _descriptionAttribute = methodInfo.GetCustomAttribute<DescriptionAttribute>();

        var options = new AIFunctionFactoryOptions
        {
            AdditionalProperties = _additionalProperties,
            Description = _descriptionAttribute?.Description,
            Name = _mcpServerToolAttribute?.Name ?? methodInfo.Name,
        };

        _method = method;
        AIFunction = AIFunctionFactory.Create(_method, options);
    }
}
