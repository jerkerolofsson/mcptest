using System.Text.Json;

using ModelContextProtocol.Protocol;

namespace TestBucket.AI.Xunit;

/// <summary>
/// Represents a response containing a list of content items.
/// </summary>
/// <param name="Content">The list of content items.</param>
public class ToolContentResponse(List<Content> Content)
{
    /// <summary>
    /// Processes the content as JSON and invokes the specified action with the deserialized object.
    /// </summary>
    /// <typeparam name="T">The type to which the JSON content should be deserialized.</typeparam>
    /// <param name="content">The action to invoke with the deserialized object.</param>
    /// <param name="mimeType">The MIME type to filter the content. If null, all content is considered.</param>
    /// <exception cref="Xunit.Sdk.XunitException">Thrown if no content matches the specified MIME type or if the content is empty.</exception>

    public void AsJsonContent<T>(Action<T?> content, string? mimeType = null)
    {
        Assert.NotEmpty(Content);

        var foundAny = false;

        foreach (var jsonContent in Content.Where(x => x.MimeType == mimeType || mimeType == null))
        {
            foundAny = true;

            if (jsonContent.Text is null)
            {
                Assert.Fail("Content found but no text was provided (text is null).");
            }

            T? t = JsonSerializer.Deserialize<T>(jsonContent.Text);

            content(t);
        }

        if (!foundAny)
        {
            Assert.Fail("There where no content returned with the mime type 'application/json' in the CallToolResponse");
        }
    }

    /// <summary>
    /// Verifies that the response content contains the specified text.
    /// </summary>
    /// <param name="text">The text to search for in the content.</param>
    /// <param name="comparison">The string comparison method to use.</param>
    /// <exception cref="Xunit.Sdk.XunitException">Thrown if the specified text is not found in the content.</exception>
    public void ShouldContain(string text, StringComparison comparison = StringComparison.Ordinal)
    {
        bool found = false;
        foreach(var content in Content)
        {
            if(content.Text is not null)
            {
                if(content.Text.Contains(text, comparison))
                {
                    found = true;
                }
            }
        }

        if(!found)
        {
            Assert.Fail($"The string '{text}' was not found in the response content");
        }
    }


    /// <summary>
    /// Verifies that the response content does NOT contain the specified text.
    /// </summary>
    /// <param name="text">The text to search for in the content.</param>
    /// <param name="comparison">The string comparison method to use.</param>
    /// <exception cref="Xunit.Sdk.XunitException">Thrown if the specified text is not found in the content.</exception>
    public void ShouldNotContain(string text, StringComparison comparison = StringComparison.Ordinal)
    {
        bool found = false;
        foreach (var content in Content)
        {
            if (content.Text is not null)
            {
                if (content.Text.Contains(text, comparison))
                {
                    found = true;
                }
            }
        }

        if (found)
        {
            Assert.Fail($"The string '{text}' was found in the response content");
        }
    }
}
