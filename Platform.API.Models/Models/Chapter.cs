using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// Represents a chapter within a book of the Bible.
/// </summary>
public sealed record Chapter
{
    /// <summary>
    /// USFM chapter identifier (e.g. <c>GEN.1</c>).
    /// </summary>
    [JsonPropertyName("usfm")]
    public string Usfm { get; init; } = string.Empty;

    /// <summary>Human-readable chapter reference (e.g. <c>Genesis 1</c>).</summary>
    [JsonPropertyName("human")]
    public string Human { get; init; } = string.Empty;

    /// <summary>Number of verses in this chapter for the given Bible version.</summary>
    [JsonPropertyName("verses")]
    public int VerseCount { get; init; }
}
