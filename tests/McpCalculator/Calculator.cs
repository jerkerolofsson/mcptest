namespace McpCalculator;

public class Calculator : ICalculator
{
    /// <summary>
    /// Adds two numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public double Add(double a, double b) => a + b;

    /// <summary>
    /// Subtracts two numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public double Subtract(double a, double b) => a - b;

    /// <summary>
    /// Multiplies two numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public double Multiply(double a, double b) => a * b;

    /// <summary>
    /// Divides two numbers
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    public double Divide(double a, double b) => a / b;
}
