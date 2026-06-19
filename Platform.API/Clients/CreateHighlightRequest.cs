using System.Text.Json.Serialization;

namespace Platform.API.Clients;

/// <summary>
/// Request body sent when creating a new highlight via the YouVersion Platform API.
/// </summary>
internal sealed record CreateHighlightRequest
{
    [JsonPropertyName("version_id")]
    public int VersionId { get; init; }

    [JsonPropertyName("usfm")]
    public string Usfm { get; init; } = string.Empty;

    /// <summary>Color name in lowercase, as expected by the API (e.g. "yellow").</summary>
    [JsonPropertyName("color")]
    public string Color { get; init; } = string.Empty;
}
