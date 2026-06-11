using Microsoft.Extensions.Logging;

using Platform.API.Exceptions;
using Platform.API.Models;

using System.Net.Http.Json;

namespace Platform.API.Clients;

/// <summary>
/// HTTP implementation of <see cref="IHighlightClient"/>.
/// Requires an OAuth access token delivered by <see cref="Platform.API.Http.OAuthBearerTokenHandler"/>.
/// </summary>
/// <remarks>
/// Register via <see cref="Platform.API.Extensions.ServiceCollectionExtensions.AddYouVersionOAuth"/>
/// to enable bearer token injection automatically.
/// </remarks>
internal sealed partial class HighlightClient : IHighlightClient
{
    // TODO: Confirm the exact highlight endpoint path with https://developers.youversion.com
    private const string HighlightsPath = "/v1/highlights";

    private readonly HttpClient _httpClient;
    private readonly ILogger<HighlightClient> _logger;

    public HighlightClient(HttpClient httpClient, ILogger<HighlightClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResult<Highlight>> GetHighlightsAsync(
        string? pageToken = null,
        CancellationToken cancellationToken = default)
    {
        var url = pageToken is not null
            ? $"{HighlightsPath}?page_token={System.Uri.EscapeDataString(pageToken)}"
            : HighlightsPath;

        _logger.LogDebug("Fetching highlights (pageToken={PageToken}).", pageToken);

        using var response = await _httpClient.GetAsync(url, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, url, cancellationToken).ConfigureAwait(false);

        var result = await response.Content
            .ReadFromJsonAsync<PagedResult<Highlight>>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var list = result ?? new PagedResult<Highlight>();
        _logger.LogDebug("Fetched {Count} highlight(s).", list.Data.Count);
        return list;
    }

    /// <inheritdoc />
    public async Task<Highlight> CreateHighlightAsync(
        int versionId,
        string usfm,
        HighlightColor color,
        CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Creating highlight for {Usfm} in version {VersionId} with color {Color}.", usfm, versionId, color);

        var payload = new { version_id = versionId, usfm, color = color.ToString().ToLowerInvariant() };
        using var content = JsonContent.Create(payload);
        using var response = await _httpClient.PostAsync(HighlightsPath, content, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, HighlightsPath, cancellationToken).ConfigureAwait(false);

        var highlight = await response.Content
            .ReadFromJsonAsync<Highlight>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = highlight ?? throw new YouVersionApiException(
            System.Net.HttpStatusCode.OK,
            $"Create highlight for '{usfm}' returned an empty response body.");

        _logger.LogDebug("Created highlight {HighlightId} for {Usfm}.", result.Id, usfm);
        return result;
    }

    /// <inheritdoc />
    public async Task DeleteHighlightAsync(string highlightId, CancellationToken cancellationToken = default)
    {
        var url = $"{HighlightsPath}/{System.Uri.EscapeDataString(highlightId)}";
        _logger.LogDebug("Deleting highlight {HighlightId}.", highlightId);

        using var response = await _httpClient.DeleteAsync(url, cancellationToken).ConfigureAwait(false);
        await EnsureSuccessAsync(response, url, cancellationToken).ConfigureAwait(false);

        _logger.LogDebug("Deleted highlight {HighlightId}.", highlightId);
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private async Task EnsureSuccessAsync(HttpResponseMessage response, string url, CancellationToken ct)
    {
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(ct).ConfigureAwait(false);
            _logger.LogError("Highlight API request to '{Url}' failed with HTTP {StatusCode} {ReasonPhrase}.", url, (int)response.StatusCode, response.ReasonPhrase);
            throw new YouVersionApiException(
                response.StatusCode,
                $"Highlight API request to '{url}' failed with status {(int)response.StatusCode} ({response.ReasonPhrase}).",
                body);
        }
    }
}
