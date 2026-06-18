using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Platform.API.Models;

/// <summary>
/// A paginated result envelope returned by list endpoints.
/// </summary>
/// <typeparam name="T">The type of item in the result set.</typeparam>
public sealed record PagedResult<T>
{
    /// <summary>Gets the items returned in this page.</summary>
    /// <value>A read-only list of items in this page.</value>
    [JsonPropertyName("data")]
    public IReadOnlyList<T> Data { get; init; } = [];

    /// <summary>
    /// Gets the opaque token to supply as <c>page_token</c> on the next request.
    /// </summary>
    /// <value>
    /// The next-page token, or <see langword="null"/> when no further pages exist.
    /// </value>
    [JsonPropertyName("next_page_token")]
    public string? NextPageToken { get; init; }
}
