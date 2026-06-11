using Microsoft.Extensions.Logging;

using Platform.API.Exceptions;
using Platform.API.Models;

using System.Net.Http.Json;
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

        var result = await GetJsonAsync<PagedResult<BibleVersionSummary>>(url, cancellationToken)
            .ConfigureAwait(false);

        var list = result ?? new PagedResult<BibleVersionSummary>();
        _logger.LogDebug("Fetched {Count} Bible version(s) from API.", list.Data.Count);
        return list;
    }

    /// <inheritdoc />
    public async Task<BibleVersion> GetVersionAsync(int versionId, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Fetching Bible version {VersionId}.", versionId);

        var result = await GetJsonAsync<BibleVersion>($"/v1/bibles/{versionId}", cancellationToken)
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

        var books = version.Books
            .Select(usfm =>
            {
                s_bookMeta.TryGetValue(usfm, out var meta);
                return new Book { Usfm = usfm, Human = meta.Human ?? usfm, ChapterCount = meta.Chapters };
            })
            .ToList()
            .AsReadOnly();

        _logger.LogDebug("Fetched {Count} book(s) for Bible version {VersionId}.", books.Count, versionId);
        return books;
    }

    // Standard chapter counts keyed by USFM book code (66 canonical books).
    private static readonly IReadOnlyDictionary<string, (string Human, int Chapters)> s_bookMeta =
        new Dictionary<string, (string Human, int Chapters)>(StringComparer.OrdinalIgnoreCase)
        {
            { "GEN", ("Genesis", 50) },        { "EXO", ("Exodus", 40) },
            { "LEV", ("Leviticus", 27) },       { "NUM", ("Numbers", 36) },
            { "DEU", ("Deuteronomy", 34) },     { "JOS", ("Joshua", 24) },
            { "JDG", ("Judges", 21) },          { "RUT", ("Ruth", 4) },
            { "1SA", ("1 Samuel", 31) },        { "2SA", ("2 Samuel", 24) },
            { "1KI", ("1 Kings", 22) },         { "2KI", ("2 Kings", 25) },
            { "1CH", ("1 Chronicles", 29) },    { "2CH", ("2 Chronicles", 36) },
            { "EZR", ("Ezra", 10) },            { "NEH", ("Nehemiah", 13) },
            { "EST", ("Esther", 10) },          { "JOB", ("Job", 42) },
            { "PSA", ("Psalms", 150) },         { "PRO", ("Proverbs", 31) },
            { "ECC", ("Ecclesiastes", 12) },    { "SNG", ("Song of Solomon", 8) },
            { "ISA", ("Isaiah", 66) },          { "JER", ("Jeremiah", 52) },
            { "LAM", ("Lamentations", 5) },     { "EZK", ("Ezekiel", 48) },
            { "DAN", ("Daniel", 12) },          { "HOS", ("Hosea", 14) },
            { "JOL", ("Joel", 3) },             { "AMO", ("Amos", 9) },
            { "OBA", ("Obadiah", 1) },          { "JON", ("Jonah", 4) },
            { "MIC", ("Micah", 7) },            { "NAM", ("Nahum", 3) },
            { "HAB", ("Habakkuk", 3) },         { "ZEP", ("Zephaniah", 3) },
            { "HAG", ("Haggai", 2) },           { "ZEC", ("Zechariah", 14) },
            { "MAL", ("Malachi", 4) },          { "MAT", ("Matthew", 28) },
            { "MRK", ("Mark", 16) },            { "LUK", ("Luke", 24) },
            { "JHN", ("John", 21) },            { "ACT", ("Acts", 28) },
            { "ROM", ("Romans", 16) },          { "1CO", ("1 Corinthians", 16) },
            { "2CO", ("2 Corinthians", 13) },   { "GAL", ("Galatians", 6) },
            { "EPH", ("Ephesians", 6) },        { "PHP", ("Philippians", 4) },
            { "COL", ("Colossians", 4) },       { "1TH", ("1 Thessalonians", 5) },
            { "2TH", ("2 Thessalonians", 3) },  { "1TI", ("1 Timothy", 6) },
            { "2TI", ("2 Timothy", 4) },        { "TIT", ("Titus", 3) },
            { "PHM", ("Philemon", 1) },         { "HEB", ("Hebrews", 13) },
            { "JAS", ("James", 5) },            { "1PE", ("1 Peter", 5) },
            { "2PE", ("2 Peter", 3) },          { "1JN", ("1 John", 5) },
            { "2JN", ("2 John", 1) },           { "3JN", ("3 John", 1) },
            { "JUD", ("Jude", 1) },             { "REV", ("Revelation", 22) },
        };

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

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

    private async Task<T?> GetJsonAsync<T>(string relativeUrl, CancellationToken cancellationToken)
    {
        using var response = await _httpClient
            .GetAsync(relativeUrl, cancellationToken)
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            _logger.LogError("API request to '{Url}' failed with HTTP {StatusCode} {ReasonPhrase}.", relativeUrl, (int)response.StatusCode, response.ReasonPhrase);
            throw new YouVersionApiException(
                response.StatusCode,
                $"YouVersion API request to '{relativeUrl}' failed with status {(int)response.StatusCode} ({response.ReasonPhrase}).",
                body);
        }

        return await response.Content
            .ReadFromJsonAsync<T>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }
}
