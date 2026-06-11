using System;
using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// A Bible verse highlight associated with a user account.
/// </summary>
public sealed record Highlight
{
    /// <summary>Unique identifier for the highlight.</summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// USFM identifier of the highlighted verse (e.g. <c>JHN.3.16</c>).
    /// </summary>
    [JsonPropertyName("usfm")]
    public string Usfm { get; init; } = string.Empty;

    /// <summary>The Bible version id this highlight belongs to.</summary>
    [JsonPropertyName("version_id")]
    public int VersionId { get; init; }

    /// <summary>The color used for this highlight.</summary>
    [JsonPropertyName("color")]
    public HighlightColor Color { get; init; }

    /// <summary>UTC timestamp when the highlight was created.</summary>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>UTC timestamp when the highlight was last updated.</summary>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }
}
