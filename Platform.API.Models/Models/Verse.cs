using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// Represents a single verse within a Bible chapter.
/// </summary>
public sealed record Verse
{
    /// <summary>
    /// USFM verse identifier (e.g. <c>JHN.3.16</c>).
    /// </summary>
    [JsonPropertyName("usfm")]
    public string Usfm { get; init; } = string.Empty;

    /// <summary>Human-readable verse reference (e.g. <c>John 3:16</c>).</summary>
    [JsonPropertyName("human")]
    public string Human { get; init; } = string.Empty;
}
