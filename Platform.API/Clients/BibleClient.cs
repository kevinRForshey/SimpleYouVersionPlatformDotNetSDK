using Microsoft.Extensions.Logging;

using Platform.API.Exceptions;
using Platform.API.Http;
using Platform.API.Models;

using System.Text;

namespace Platform.API.Clients;

/// <summary>
/// Default implementation of <see cref="IBibleClient"/>.
/// </summary>
internal sealed class BibleClient : IBibleClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<BibleClient> _logger;

    public BibleClient(HttpClient httpClient, ILogger<BibleClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<PagedResult<BibleVersionSummary>> GetVersionsAsync(
        string languageRange = "en",
        string? pageToken = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default)
    {
        var url = BuildVersionsUrl(languageRange, pageToken, pageSize);
        _logger.LogDebug("Fetching Bible versions for language range '{LanguageRange}' (pageToken={PageToken}).", languageRange, pageToken);

        var result = await ApiRequestHelper.GetJsonAsync<PagedResult<BibleVersionSummary>>(_httpClient, url, _logger, cancellationToken)
            .ConfigureAwait(false);

        var list = result ?? new PagedResult<BibleVersionSummary>();
        _logger.LogDebug("Fetched {Count} Bible version(s) from API.", list.Data.Count);
        return list;
    }

    /// <inheritdoc />
    public async Task<BibleVersion> GetVersionAsync(int versionId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching Bible version {VersionId}.", versionId);

        var result = await ApiRequestHelper.GetJsonAsync<BibleVersion>(_httpClient, $"/v1/bibles/{versionId}", _logger, cancellationToken)
            .ConfigureAwait(false);

        var version = result ?? throw new YouVersionApiException(
            System.Net.HttpStatusCode.NotFound,
            $"Bible version {versionId} was not found or returned an empty response.");

        _logger.LogDebug("Fetched Bible version {VersionId} ({Abbreviation}).", versionId, version.Abbreviation);
        return version;
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Book>> GetBooksAsync(int versionId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching books for Bible version {VersionId}.", versionId);

        var version = await GetVersionAsync(versionId, cancellationToken).ConfigureAwait(false);
        var books = BuildBookList(version);

        _logger.LogDebug("Fetched {Count} book(s) for Bible version {VersionId}.", books.Count, versionId);
        return books;
    }

    /// <inheritdoc />
    public Task<IReadOnlyList<Book>> GetBooksAsync(BibleVersion version)
        => Task.FromResult<IReadOnlyList<Book>>(BuildBookList(version));

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static IReadOnlyList<Book> BuildBookList(BibleVersion version)
        => version.Books.Select(BibleBookCatalog.FromUsfm).ToList().AsReadOnly();

    private static string BuildVersionsUrl(string languageRange, string? pageToken, int? pageSize)
    {
        var sb = new StringBuilder("/v1/bibles?language_ranges[]=");
        sb.Append(Uri.EscapeDataString(languageRange));

        if (pageToken is not null)
        {
            sb.Append("&page_token=");
            sb.Append(Uri.EscapeDataString(pageToken));
        }

        if (pageSize.HasValue)
        {
            sb.Append("&page_size=");
            sb.Append(pageSize.Value);
        }

        return sb.ToString();
    }

    }
