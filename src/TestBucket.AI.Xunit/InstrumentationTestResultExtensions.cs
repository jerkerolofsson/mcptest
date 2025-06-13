using System.Diagnostics;

using TestBucket.AI.Xunit.Instrumentation;

namespace TestBucket.AI.Xunit;

/// <summary>
/// Extensions for tests using InstrumentationChatClient and InstrumentationTestResult.
/// </summary>
public static class InstrumentationTestResultExtensions
{
    /// <summary>
    /// Verifies that the <see cref="InstrumentationTestResult"/> indicates that a specific tool was invoked a specified number of times.
    /// </summary>
    /// <param name="testResult"></param>
    /// <param name="toolName"></param>
    /// <param name="times"></param>
    public static void ContainsFunctionCall(this InstrumentationTestResult testResult, string toolName, int times=1)
    {
        var count = testResult.FunctionCalls.Count(f => f.Name == toolName);
        if (count == 0)
        {
            Assert.Fail($"Tool '{toolName}' was not invoked during the test run.");
        }
        else if (count != times)
        {
            Assert.Fail($"Tool '{toolName}' was invoked {count} times, expected {times} times.");
        }
    }

    /// <summary>
    /// Verifies that the <see cref="InstrumentationTestResult"/> indicates a successful test run.
    /// 
    /// - No exceptions from tool/function calls
    /// - No activities with a status other than <see cref="ActivityStatusCode.Ok"/>.
    /// 
    /// </summary>
    /// <param name="testResult"></param>
    public static void ShouldBeSuccess(this InstrumentationTestResult testResult)
    {
        var failingActivities = testResult.Activities.Where(a => a.Status != ActivityStatusCode.Ok).ToList();

        // If there are multiple failing activities, we only log the first one in the assert so add warnings for the rest.
        foreach (var ex in testResult.FunctionCallExceptions)
        {
            TestContext.Current.AddWarning($"Function call exception: {ex.Message}");
        }
        foreach (var activity in failingActivities)
        {
            TestContext.Current.AddWarning($"Activity did not complete successfully: DisplayName={activity.DisplayName}, Status={activity.Status}");
        }

        if (failingActivities.Count > 0)
        {
            Assert.Fail($"""
                Test run contains {failingActivities.Count} activities that did not complete successfully. 
                See warnings for details. 
                
                First={failingActivities[0].DisplayName}
                """);
                
        }
        if (testResult.FunctionCallExceptions.Count > 0)
        {
            Assert.Fail($"""
                Test run contains {testResult.FunctionCallExceptions.Count} function/tool calls that threw exceptions.
                See warnings for details. 

                {testResult.FunctionCallExceptions[0].ToString()}
                """);
        }
    }
}
