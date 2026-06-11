namespace Platform.API.Models;

/// <summary>
/// Controls the content format returned for a Bible passage.
/// </summary>
public enum PassageFormat
{
    /// <summary>Plain-text content with no markup.</summary>
    Text,

    /// <summary>HTML content styled for the YouVersion Bible stylesheet.</summary>
    Html
}
