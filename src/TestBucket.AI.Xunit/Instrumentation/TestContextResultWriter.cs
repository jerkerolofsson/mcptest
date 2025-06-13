using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TestBucket.Traits.Xunit;

namespace TestBucket.AI.Xunit.Instrumentation;

/// <summary>
/// Augments metrics and other details to the TestContext as attachments so it is included in the xml reports
/// when running tests with xUnit.
/// </summary>
internal static class TestContextResultWriter
{
    public static void AttachInstrumentationTestResult(this ITestContext testContext, InstrumentationTestResult result)
    {
        

        //testContext.AddMetric();
    }
}
