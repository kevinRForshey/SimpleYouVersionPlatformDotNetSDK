using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Platform.API.Exceptions;
using Platform.API.Models;

namespace Platform.API.Clients;

/// <summary>
/// Default implementation of <see cref="IPassageClient"/> backed by the YouVersion Platform REST API.
/// </summary>
internal sealed partial class PassageClient : IPassageClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<PassageClient> _logger;

    public PassageClient(HttpClient httpClient, ILogger<PassageClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<Passage> GetPassageAsync(
        int versionId,
        string usfm,
        PassageRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        var resolvedOptions = options ?? PassageRequestOptions.Default;
        var url = BuildPassageUrl(versionId, usfm, resolvedOptions);

        _logger.LogDebug("Fetching passage {Usfm} from version {VersionId} (format={Format}).", usfm, versionId, resolvedOptions.Format);

        using var response = await _httpClient
            .GetAsync(url, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogError("Failed to fetch passage {Usfm} from version {VersionId}: HTTP {StatusCode} {ReasonPhrase}.", usfm, versionId, (int)response.StatusCode, response.ReasonPhrase);
            throw new YouVersionApiException(
                response.StatusCode,
                $"YouVersion API request for passage '{usfm}' (version {versionId}) failed with status {(int)response.StatusCode} ({response.ReasonPhrase}).",
                body);
        }

        var passage = await response.Content
            .ReadFromJsonAsync<Passage>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);

        var result = passage ?? throw new YouVersionApiException(
            System.Net.HttpStatusCode.OK,
            $"The API returned an empty body for passage '{usfm}' (version {versionId}).");

        _logger.LogDebug("Fetched passage {Usfm} from version {VersionId}.", usfm, versionId);
        return result;
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static string BuildPassageUrl(int versionId, string usfm, PassageRequestOptions options)
    {
        var sb = new StringBuilder("/v1/bibles/");
        sb.Append(versionId);
        sb.Append("/passages/");
        sb.Append(Uri.EscapeDataString(usfm));
        sb.Append("?format=");
        sb.Append(options.Format == PassageFormat.Html ? "html" : "text");

        if (options.IncludeHeadings)
            sb.Append("&include_headings=true");

        if (options.IncludeNotes)
            sb.Append("&include_notes=true");

        return sb.ToString();
    }
}
