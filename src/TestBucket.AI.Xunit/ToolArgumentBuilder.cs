namespace TestBucket.AI.Xunit;

/// <summary>
/// A builder class for constructing a dictionary of tool arguments.
/// </summary>
public class ToolArgumentBuilder
{
    private readonly Dictionary<string, object?> _dictionary = [];

    /// <summary>
    /// Adds or updates an argument with a string value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The string value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, string value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates an argument with an integer value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The integer value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, int value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates an argument with a double value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The double value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, double value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates an argument with a float value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The float value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, float value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates an argument with a decimal value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The decimal value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, decimal value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates an argument with a short value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The short value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, short value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Adds or updates an argument with a byte value.
    /// </summary>
    /// <param name="key">The key of the argument.</param>
    /// <param name="value">The byte value of the argument.</param>
    /// <returns>The current instance of <see cref="ToolArgumentBuilder"/>.</returns>
    public ToolArgumentBuilder WithArgument(string key, byte value)
    {
        _dictionary[key] = value;
        return this;
    }

    /// <summary>
    /// Builds and returns the dictionary of arguments.
    /// </summary>
    /// <returns>A dictionary containing all the arguments added to the builder.</returns>
    public Dictionary<string, object?> Build() => AsDictionary();

    /// <summary>
    /// Returns the dictionary of arguments without building a new instance.
    /// </summary>
    /// <returns>The dictionary containing all the arguments.</returns>
    public Dictionary<string, object?> AsDictionary() => _dictionary;
}