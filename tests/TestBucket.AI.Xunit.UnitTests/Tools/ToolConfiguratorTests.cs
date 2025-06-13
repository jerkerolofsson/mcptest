using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using McpCalculator;

using Microsoft.Extensions.DependencyInjection;

using TestBucket.AI.Xunit.Tools;

namespace TestBucket.AI.Xunit.UnitTests.Tools;

/// <summary>
/// Contains unit tests for the <see cref="ToolConfigurator"/> class, verifying the registration and discovery of MCP server tools.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Tools")]
[Component("McpServerToolAdapter")]
public class ToolConfiguratorTests
{
    /// <summary>
    /// Tests that <see cref="ToolConfigurator.AddMcpServerToolsFromAssembly(System.Reflection.Assembly)"/> adds tools from a specified assembly.
    /// </summary>
    [Fact]
    public void AddMcpServerToolsFromAssembly_WithAssemblyContainingTools_ShouldAddToolsFromAssembly()
    {
        // Arrange
        var configurator = new ToolConfigurator();
        configurator.Services = CreateCalculatorServiceProvider();

        // Act
        configurator.AddMcpServerToolsFromAssembly(typeof(McpCalculator.CalculatorMcp).Assembly);

        // Assert
        var tools = configurator.Build();
        Assert.NotEmpty(tools);
        Assert.NotEmpty(tools.Where(t => t.Name == "Add").ToArray());
        Assert.NotEmpty(tools.Where(t => t.Name == "Subtract").ToArray());
    }

    /// <summary>
    /// Creates a <see cref="ServiceProvider"/> with calculator services registered.
    /// </summary>
    /// <returns>A <see cref="ServiceProvider"/> instance with <see cref="ICalculator"/> and <see cref="Calculator"/> registered.</returns>
    private static ServiceProvider CreateCalculatorServiceProvider()
    {
        return new ServiceCollection().AddSingleton<ICalculator, Calculator>().BuildServiceProvider();
    }

    /// <summary>
    /// Tests that <see cref="ToolConfigurator.AddMcpServerToolsFromType(Type)"/> adds tools from a class containing MCP tools.
    /// </summary>
    [Fact]
    public void AddMcpServerToolsFromType_WithClassContainingMcpTools_ShouldAddToolsFromClass()
    {
        // Arrange
        var configurator = new ToolConfigurator();
        configurator.Services = CreateCalculatorServiceProvider();

        // Act
        configurator.AddMcpServerToolsFromType(typeof(McpCalculator.CalculatorMcp));

        // Assert
        var tools = configurator.Build();
        Assert.NotEmpty(tools);
        Assert.NotEmpty(tools.Where(t => t.Name == "Add").ToArray());
        Assert.NotEmpty(tools.Where(t => t.Name == "Subtract").ToArray());
    }

    /// <summary>
    /// Tests that <see cref="ToolConfigurator.AddMcpServerToolsFromType(Type)"/> does not add tools when the class does not contain any MCP tools.
    /// </summary>
    [Fact]
    public void AddMcpServerToolsFromType_WithClassNotContainingTools_NoToolsAdded()
    {
        // Arrange
        var configurator = new ToolConfigurator();
        configurator.Services = CreateCalculatorServiceProvider();

        // Act
        configurator.AddMcpServerToolsFromType(typeof(ToolConfiguratorTests));

        // Assert
        var tools = configurator.Build();
        Assert.Empty(tools);
    }

    /// <summary>
    /// Tests that <see cref="ToolConfigurator.AddMcpServerToolsFromType(Type)"/> throws an <see cref="ArgumentException"/> when provided a non-class type.
    /// </summary>
    [Fact]
    public void AddMcpServerToolsFromType_WithNonClassType_ThrowsArgumentException()
    {
        // Arrange
        var configurator = new ToolConfigurator();
        configurator.Services = CreateCalculatorServiceProvider();

        // Act
        Assert.Throws<ArgumentException>(() =>
        {
            configurator.AddMcpServerToolsFromType(typeof(uint));
        });
    }
}