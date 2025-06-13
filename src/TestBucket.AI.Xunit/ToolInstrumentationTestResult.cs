using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.AI;

using TestBucket.AI.Xunit.Instrumentation;

namespace TestBucket.AI.Xunit;
public record class ToolInstrumentationTestResult
{
    private readonly List<FunctionCallContent> _functionCalls;
    private readonly InstrumentationTestResult _result;

    public ToolInstrumentationTestResult(List<FunctionCallContent> functionCalls, InstrumentationTestResult result)
    {
        _functionCalls = functionCalls;
        _result = result;
    }


    public ToolInstrumentationTestResult WithArgument(string argumentName, object? expectedValue)
    {
        foreach(var functionCall in _functionCalls)
        {
            var arguments = functionCall.Arguments;
            if(arguments is null || arguments.Count == 0)
            {
                Assert.Fail($"The call to '{functionCall.Name}' had no arguments");
            }
            if(!arguments.TryGetValue(argumentName, out var argumentValue))
            {
                Assert.Fail($"The call to '{functionCall.Name}' did not have an argument named '{argumentName}'");
            }
            if(argumentValue is null && expectedValue is null)
            {
                continue;
            }
            if(expectedValue is null && argumentValue is not null)
            {
                Assert.Fail($"Expected the call to '{functionCall.Name}' have '{argumentName}'=null but expected it was '{argumentValue}'");
            }
            if(expectedValue is not null && !expectedValue.Equals(argumentValue))
            {
                if(argumentValue is null)
                {
                    Assert.Fail($"Expected the call to '{functionCall.Name}' have '{argumentName}'='{expectedValue}' but expected it was null");
                }

                // If it failed because the types are different, and the provided
                // argument is a string, try to convert it to the expected type.
                var providedArgumentType = argumentValue.GetType();
                var expectedArgumentType = expectedValue.GetType(); 
                if(expectedArgumentType != typeof(JsonElement) && argumentValue is JsonElement jsonElement)
                {
                    if (expectedArgumentType.IsValueType)
                    {
                        try
                        {
                            argumentValue = JsonSerializer.Deserialize(jsonElement.GetRawText(), expectedArgumentType);
                        }
                        catch
                        {
                            // It is up to the user to provide a comparable type for complex objects that cannot be deserialized..
                        }
                    }
                }

                if (expectedArgumentType.IsValueType && expectedArgumentType != providedArgumentType)
                {
                    // Convert the string to the expected type
                    argumentValue = Convert.ChangeType(argumentValue, expectedArgumentType, CultureInfo.InvariantCulture);
                }

                if (!expectedValue.Equals(argumentValue))
                {
                    Assert.Fail($"Expected the call to '{functionCall.Name}' have '{argumentName}'='{expectedValue}' but expected it was '{argumentValue}'");
                }
            }
        }
        return this;
    }
}
