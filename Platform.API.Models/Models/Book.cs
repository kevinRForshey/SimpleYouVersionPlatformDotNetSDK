using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// Represents a book of the Bible within a specific version.
/// </summary>
public sealed record Book
{
    /// <summary>USFM book code (e.g. <c>GEN</c>, <c>MAT</c>, <c>REV</c>).</summary>
    [JsonPropertyName("usfm")]
    public string Usfm { get; init; } = string.Empty;

    /// <summary>Human-readable name of the book (e.g. <c>Genesis</c>).</summary>
    [JsonPropertyName("human")]
    public string Human { get; init; } = string.Empty;

    /// <summary>Number of chapters in this book for the given Bible version.</summary>
    [JsonPropertyName("chapters")]
    public int ChapterCount { get; init; }
}
