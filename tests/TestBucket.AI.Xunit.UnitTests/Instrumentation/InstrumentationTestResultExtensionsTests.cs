using System.Diagnostics;

using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;

using Xunit.Sdk;

namespace TestBucket.AI.Xunit.UnitTests.Instrumentation;

/// <summary>
/// Unit tests for <see cref="InstrumentationTestResultExtensions"/> extension methods,
/// verifying correct behavior for function call tracking and result validation.
/// </summary>
[UnitTest]
[EnrichedTest]
[FunctionalTest]
[Feature("Instrumentation")]
[Component("Instrumentation")]
public class InstrumentationTestResultExtensionsTests
{
    /// <summary>
    /// Verifies that ContainsFunctionCall
    /// returns a result when the specified tool is called exactly once.
    /// </summary>
    [Fact]
    public void ContainsFunctionCallOnce_ReturnsResult_WhenToolIsCalledOnce()
    {
        var testResult = new InstrumentationTestResult();
        var functionCall = new FunctionCallContent("myCallId", "myTool", new Dictionary<string, object?>());
        testResult.AddFunctionCall(functionCall);

        var result = testResult.ContainsFunctionCall("myTool", 1);

        Assert.NotNull(result);
    }

    /// <summary>
    /// Verifies that ContainsFunctionCall
    /// returns a result when the specified tool is called exactly twice.
    /// </summary>
    [Fact]
    public void ContainsFunctionCallTwice_ReturnsResult_WhenToolIsCalledTwice()
    {
        var testResult = new InstrumentationTestResult();
        testResult.AddFunctionCall(new FunctionCallContent("myCallId", "myTool", null));
        testResult.AddFunctionCall(new FunctionCallContent("myCallId", "myTool", null));

        var result = testResult.ContainsFunctionCall("myTool", 2);

        Assert.NotNull(result);
    }

    /// <summary>
    /// Verifies that ContainsFunctionCall
    /// throws when the tool is called fewer times than expected.
    /// </summary>
    [Fact]
    public void ContainsFunctionCallTwice_Fails_WhenToolIsCalledOnce()
    {
        var testResult = new InstrumentationTestResult();
        testResult.AddFunctionCall(new FunctionCallContent("myCallId", "myTool", null));

        var ex = Assert.Throws<FailException>(() =>
        {
            testResult.ContainsFunctionCall("myTool", 2);
        });
    }

    /// <summary>
    /// Verifies that ContainsFunctionCall
    /// throws when the specified tool was not called at all.
    /// </summary>
    [Fact]
    public void ContainsFunctionCall_Throws_WhenToolNotCalled()
    {
        var testResult = new InstrumentationTestResult();

        var ex = Assert.Throws<FailException>(() =>
        {
            testResult.ContainsFunctionCall("missingTool");
        });

        Assert.Contains("Tool 'missingTool' was not invoked", ex.Message);
    }

    /// <summary>
    /// Verifies that ContainsFunctionCall
    /// throws when the tool is called the wrong number of times.
    /// </summary>
    [Fact]
    public void ContainsFunctionCall_Throws_WhenToolCalledWrongNumberOfTimes()
    {
        var testResult = new InstrumentationTestResult();
        var functionCall = new FunctionCallContent("myCallId", "myTool", null);

        testResult.AddFunctionCall(functionCall);
        var ex = Assert.Throws<FailException>(() =>
        {
            testResult.ContainsFunctionCall("myTool", 2);
        });

        Assert.Contains("was invoked 1 times, expected 2 times", ex.Message);
    }

    /// <summary>
    /// Verifies that ShouldBeSuccess
    /// when there are no failures in activities or function calls.
    /// </summary>
    [Fact]
    public void ShouldBeSuccess_DoesNotThrow_WhenNoFailures()
    {
        var testResult = new InstrumentationTestResult();
        var activity = new Activity("test") { DisplayName = "TestActivity" };
        activity.SetCustomProperty("Status", ActivityStatusCode.Ok);
        testResult.AddActivity(activity);

        // No activities or exceptions
        testResult.ShouldBeSuccess();
    }

    /// <summary>
    /// Verifies that ShouldBeSuccess
    /// when an activity has an error status.
    /// </summary>
    [Fact]
    public void ShouldBeSuccess_Throws_WhenActivityHasError()
    {
        var testResult = new InstrumentationTestResult();
        var activity = new Activity("test") { DisplayName = "TestActivity" };
        activity.SetStatus(ActivityStatusCode.Error);

        testResult.AddActivity(activity);

        var ex = Assert.Throws<FailException>(() =>
        {
            testResult.ShouldBeSuccess();
        });

        Assert.Contains("activities that did not complete successfully", ex.Message);
    }

    /// <summary>
    /// Verifies that ShouldBeSuccess throws
    /// when a function call exception occured.
    /// </summary>
    [Fact]
    public void ShouldBeSuccess_Throws_WhenFunctionCallExceptionExists()
    {
        var testResult = new InstrumentationTestResult();
        var exception = new InvalidOperationException("fail!");
        testResult.AddFunctionCallException(exception);

        var ex = Assert.Throws<FailException>(() =>
        {
            testResult.ShouldBeSuccess();
        });

        Assert.Contains("function/tool calls that threw exceptions", ex.Message);
    }
}