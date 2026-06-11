using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// A paginated result envelope returned by list endpoints.
/// </summary>
/// <typeparam name="T">The type of item in the result set.</typeparam>
public sealed record PagedResult<T>
{
    /// <summary>The items returned in this page.</summary>
    [JsonPropertyName("data")]
    public IReadOnlyList<T> Data { get; init; } = [];

    /// <summary>
    /// Opaque token to supply as <c>page_token</c> on the next request.
    /// <see langword="null"/> when no further pages exist.
    /// </summary>
    [JsonPropertyName("next_page_token")]
    public string? NextPageToken { get; init; }
}
