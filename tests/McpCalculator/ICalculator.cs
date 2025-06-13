namespace McpCalculator;

/// <summary>
/// Defines basic arithmetic operations for a calculator.
/// </summary>
public interface ICalculator
{
    /// <summary>
    /// Adds two double-precision numbers.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The sum of <paramref name="a"/> and <paramref name="b"/>.</returns>
    double Add(double a, double b);

    /// <summary>
    /// Divides one double-precision number by another.
    /// </summary>
    /// <param name="a">The dividend.</param>
    /// <param name="b">The divisor.</param>
    /// <returns>The result of dividing <paramref name="a"/> by <paramref name="b"/>.</returns>
    double Divide(double a, double b);

    /// <summary>
    /// Multiplies two double-precision numbers.
    /// </summary>
    /// <param name="a">The first operand.</param>
    /// <param name="b">The second operand.</param>
    /// <returns>The product of <paramref name="a"/> and <paramref name="b"/>.</returns>
    double Multiply(double a, double b);

    /// <summary>
    /// Subtracts one double-precision number from another.
    /// </summary>
    /// <param name="a">The value to subtract from.</param>
    /// <param name="b">The value to subtract.</param>
    /// <returns>The result of <paramref name="a"/> minus <paramref name="b"/>.</returns>
    double Subtract(double a, double b);
}