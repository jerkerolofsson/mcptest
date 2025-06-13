using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;

using Xunit.Sdk;

namespace TestBucket.AI.Xunit.UnitTests.Instrumentation;

/// <summary>
/// Unit tests for <see cref="ToolInstrumentationTestResult"/> extension methods,
/// verifying correct behavior for function call tracking and result validation.
/// </summary>
[UnitTest]
[Feature("Instrumentation")]
[Component("ToolInstrumentationTestResult")]
[EnrichedTest]
[FunctionalTest]
public class ToolInstrumentationTestResultTests
{
    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and an integer expected argument is successful.
    /// </summary>
    [Fact]
    public void WithArgument_IsSuccessful_With_StringArgument_And_IntExpected()
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        int expectedValue = 1;
        args["a"] = expectedValue.ToString(); // Set as string to verify that it will be converted successfully

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", times: 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with an integer argument and an integer expected argument is successful.
    /// </summary>
    [Fact]
    public void WithArgument_IsSuccessful_With_IntArgument_And_IntExpected()
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = 1;

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        var result = testResult.ContainsFunctionCall("myTool", 1);
        result.WithArgument("a", 1);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> throws an exception when the expected argument is missing.
    /// </summary>
    [Fact]
    public void WithArgument_ThrowsException_WithMissingArgument()
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = 1;

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        var result = testResult.ContainsFunctionCall("myTool", 1);
        Assert.Throws<FailException>(() =>
        {
            result.WithArgument("b", 1);
        });
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a double expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected double value to validate.</param>
    [Theory]
    [InlineData(double.MinValue)]
    [InlineData(-1.0)]
    [InlineData(0.0)]
    [InlineData(1.0)]
    [InlineData(double.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_DoubleExpected(double expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString("R", System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a float expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected float value to validate.</param>
    [Theory]
    [InlineData(float.MinValue)]
    [InlineData(-1.0f)]
    [InlineData(0.0f)]
    [InlineData(1.0f)]
    [InlineData(float.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_FloatExpected(float expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString("R", System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a long expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected long value to validate.</param>
    [Theory]
    [InlineData(long.MinValue)]
    [InlineData(-1L)]
    [InlineData(0L)]
    [InlineData(1L)]
    [InlineData(long.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_LongExpected(long expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a byte expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected byte value to validate.</param>
    [Theory]
    [InlineData(byte.MinValue)]
    [InlineData((byte)1)]
    [InlineData(byte.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_ByteExpected(byte expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a short expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected short value to validate.</param>
    [Theory]
    [InlineData(short.MinValue)]
    [InlineData((short)-1)]
    [InlineData((short)0)]
    [InlineData((short)1)]
    [InlineData(short.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_ShortExpected(short expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a uint expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected uint value to validate.</param>
    [Theory]
    [InlineData(uint.MinValue)]
    [InlineData((uint)1)]
    [InlineData(uint.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_UIntExpected(uint expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a ushort expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected ushort value to validate.</param>
    [Theory]
    [InlineData(ushort.MinValue)]
    [InlineData((ushort)1)]
    [InlineData(ushort.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_UShortExpected(ushort expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString(System.Globalization.CultureInfo.InvariantCulture);

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a char expected argument is successful.
    /// </summary>
    /// <param name="expectedValue">The expected char value to validate.</param>
    [Theory]
    [InlineData(char.MinValue)]
    [InlineData('A')]
    [InlineData(char.MaxValue)]
    public void WithArgument_IsSuccessful_With_StringArgument_And_CharExpected(char expectedValue)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = expectedValue.ToString();

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }

    /// <summary>
    /// Verifies that <c>WithArgument</c> called from <c>ContainsFunctionCall</c> with a string argument and a decimal expected argument is successful.
    /// </summary>
    /// <param name="value">The string representation of the expected decimal value.</param>
    [Theory]
    [InlineData("-79228162514264337593543950335")]
    [InlineData("-1")]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("79228162514264337593543950335")]
    public void WithArgument_IsSuccessful_With_StringArgument_And_DecimalExpected(string value)
    {
        var testResult = new InstrumentationTestResult();
        Dictionary<string, object?> args = new();
        args["a"] = value;

        var functionCall = new FunctionCallContent("myCallId", "myTool", args);
        testResult.AddFunctionCall(functionCall);

        decimal expectedValue = (decimal)Convert.ChangeType(value, typeof(decimal), System.Globalization.CultureInfo.InvariantCulture);
        testResult.ContainsFunctionCall("myTool", 1).WithArgument("a", expectedValue);
    }
}