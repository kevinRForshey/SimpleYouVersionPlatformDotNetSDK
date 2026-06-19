using System;
using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// A Bible verse highlight associated with a user account.
/// </summary>
public sealed record Highlight
{
    /// <summary>Gets the unique identifier for this highlight.</summary>
    /// <value>The unique highlight identifier.</value>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>Gets the USFM identifier of the highlighted verse (e.g. <c>JHN.3.16</c>).</summary>
    /// <value>The USFM identifier for the highlighted verse.</value>
    [JsonPropertyName("usfm")]
    public string Usfm { get; init; } = string.Empty;

    /// <summary>Gets the Bible version identifier this highlight belongs to.</summary>
    /// <value>The numeric Bible version identifier.</value>
    [JsonPropertyName("version_id")]
    public int VersionId { get; init; }

    /// <summary>Gets the color used for this highlight.</summary>
    /// <value>One of the <see cref="HighlightColor"/> values.</value>
    [JsonPropertyName("color")]
    public HighlightColor Color { get; init; }

    /// <summary>Gets the UTC timestamp when the highlight was created.</summary>
    /// <value>The UTC creation timestamp.</value>
    [JsonPropertyName("created_at")]
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Gets the UTC timestamp when the highlight was last updated.</summary>
    /// <value>The UTC last-updated timestamp.</value>
    [JsonPropertyName("updated_at")]
    public DateTimeOffset UpdatedAt { get; init; }
}
