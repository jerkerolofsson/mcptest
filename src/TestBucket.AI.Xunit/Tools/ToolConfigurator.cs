using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using ModelContextProtocol.Server;

namespace TestBucket.AI.Xunit.Tools;

/// <summary>
/// Configures and registers AI function tools for use in the test environment.
/// </summary>
internal class ToolConfigurator : IToolConfigurator
{
    private readonly List<IAIFunctionAdapter> _tools = [];

    /// <summary>
    /// Gets or sets the service provider used for dependency injection when creating tool instances.
    /// </summary>
    internal IServiceProvider? Services { get; set; }

    /// <summary>
    /// Adds a tool by wrapping the specified delegate.
    /// </summary>
    /// <param name="method">The delegate representing the tool method.</param>
    public void Add(Delegate method)
    {
        _tools.Add(new McpServerToolAdapter(method));
    }

    /// <summary>
    /// Adds a tool by wrapping the specified method and optional target instance.
    /// </summary>
    /// <param name="method">The method information to wrap.</param>
    /// <param name="target">The target object for instance methods, or <c>null</c> for static methods.</param>
    public void Add(MethodInfo method, object? target)
    {
        _tools.Add(new McpServerToolAdapter(method, target));
    }

    /// <summary>
    /// Scans the provided assembly for types marked with <see cref="McpServerToolTypeAttribute"/>
    /// and adds all their methods as tools.
    /// </summary>
    /// <param name="toolAssembly">The assembly to scan for tool types.</param>
    public void AddMcpServerToolsFromAssembly(Assembly toolAssembly)
    {
        var toolTypes = toolAssembly.GetTypes().Where(x => x.GetCustomAttribute<McpServerToolTypeAttribute>() is not null);
        foreach (var toolType in toolTypes)
        {
            AddMcpServerToolsFromType(toolType);
        }
    }

    /// <summary>
    /// Adds all methods from the specified generic type as tools.
    /// </summary>
    /// <typeparam name="T">The type whose methods will be added as tools.</typeparam>
    public void AddMcpServerToolsFromType<T>()
    {
        AddMcpServerToolsFromType(typeof(T));
    }

    /// <summary>
    /// Adds all methods from the specified type as tools.
    /// </summary>
    /// <param name="toolType">The type whose methods will be added as tools.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="toolType"/> is not a class.</exception>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="Services"/> is not initialized.</exception>
    public void AddMcpServerToolsFromType(Type toolType)
    {
        if (!toolType.IsClass)
        {
            throw new ArgumentException($"Expected {nameof(toolType)} to be a class");
        }

        var toolTypeAttribute = toolType.GetCustomAttribute<McpServerToolTypeAttribute>();
        if(toolTypeAttribute is null)
        {
            return;
        }

        if (Services is null)
        {
            throw new InvalidOperationException("Services is not initialized");
        }

        object? instance = null;

        MethodInfo[] methods = toolType.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (MethodInfo toolMethod in methods)
        {
            if (toolMethod.IsStatic)
            {
                Add(toolMethod, null);
            }
            else
            {
                instance ??= ActivatorUtilities.CreateInstance(Services, toolType);
                Add(toolMethod, instance);
            }
        }
    }

    /// <summary>
    /// Builds and returns a read-only list of all registered AI function adapters.
    /// </summary>
    /// <returns>A read-only list of <see cref="IAIFunctionAdapter"/> instances.</returns>
    internal IReadOnlyList<IAIFunctionAdapter> Build()
    {
        var allTools = _tools.ToList();
        return allTools;
    }
}