using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// Full metadata for a single Bible version, including its available books.
/// </summary>
public sealed record BibleVersion
{
    /// <summary>Unique numeric identifier for the Bible version.</summary>
    [JsonPropertyName("id")]
    public int Id { get; init; }

    /// <summary>Short abbreviation for the version (e.g. <c>NIV</c>, <c>BSB</c>).</summary>
    [JsonPropertyName("abbreviation")]
    public string Abbreviation { get; init; } = string.Empty;

    /// <summary>Abbreviation localized to the version's language.</summary>
    [JsonPropertyName("localized_abbreviation")]
    public string LocalizedAbbreviation { get; init; } = string.Empty;

    /// <summary>Full title of the Bible version.</summary>
    [JsonPropertyName("title")]
    public string Title { get; init; } = string.Empty;

    /// <summary>Title localized to the version's language.</summary>
    [JsonPropertyName("localized_title")]
    public string LocalizedTitle { get; init; } = string.Empty;

    /// <summary>BCP-47 language tag (e.g. <c>en</c>, <c>es</c>).</summary>
    [JsonPropertyName("language_tag")]
    public string LanguageTag { get; init; } = string.Empty;

    /// <summary>Copyright statement to display alongside any Bible text from this version.</summary>
    [JsonPropertyName("copyright")]
    public string Copyright { get; init; } = string.Empty;

    /// <summary>Short promotional description of the version.</summary>
    [JsonPropertyName("promotional_content")]
    public string? PromotionalContent { get; init; }

    /// <summary>URL to the publisher's website, if available.</summary>
    [JsonPropertyName("publisher_url")]
    public string? PublisherUrl { get; init; }

    /// <summary>
    /// USFM book codes available in this version (e.g. <c>GEN</c>, <c>JHN</c>).
    /// </summary>
    [JsonPropertyName("books")]
    public IReadOnlyList<string> Books { get; init; } = [];

    /// <summary>Deep link to this version on YouVersion (bible.com).</summary>
    [JsonPropertyName("youversion_deep_link")]
    public string? YouVersionDeepLink { get; init; }
}
