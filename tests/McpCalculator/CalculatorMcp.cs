using System.ComponentModel;

using ModelContextProtocol.Server;

namespace McpCalculator;

[McpServerToolType]
public class CalculatorMcp
{
    private readonly ICalculator _calculator;

    /// <summary>
    /// Initializes a new instance of the <see cref="CalculatorMcp"/> class.
    /// </summary>
    /// <param name="calculator">The calculator to use for operations.</param>
    public CalculatorMcp(ICalculator calculator)
    {
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
    }

    /// <summary>
    /// Adds two numbers using the provided calculator.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The sum of the two numbers.</returns>
    [McpServerTool(Name = "Add"), Description("Adds two numbers")] 
    public double Add(double a, double b) => _calculator.Add(a, b);

    /// <summary>
    /// Divides one number by another using the provided calculator.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of dividing <paramref name="a"/> by <paramref name="b"/>.</returns>
    [McpServerTool(Name = "Divide"), Description("Divides two numbers")] 
    public double Divide(double a, double b) => _calculator.Divide(a, b);

    /// <summary>
    /// Multiplies two numbers using the provided calculator.
    /// </summary>
    /// <param name="a">The first number.</param>
    /// <param name="b">The second number.</param>
    /// <returns>The product of the two numbers.</returns>
    [McpServerTool(Name = "Multiply"), Description("Multiplies two numbers")] 
    public double Multiply(double a, double b) => _calculator.Multiply(a, b);

    /// <summary>
    /// Subtracts one number from another using the provided calculator.
    /// </summary>
    /// <param name="a">The value to subtract from.</param>
    /// <param name="b">The value to subtract.</param>
    /// <returns>The result of <paramref name="a"/> minus <paramref name="b"/>.</returns>
    [McpServerTool(Name = "Subtract"), Description("Subtracts two numbers")] 
    public double Subtract(double a, double b) => _calculator.Subtract(a, b);
}