using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Platform.API.Models;

namespace Platform.API.Clients;

/// <summary>
/// Provides discovery and metadata operations for Bible versions and their structure.
/// </summary>
public interface IBibleClient
{
    /// <summary>
    /// Returns a paginated list of Bible versions visible to the current app key.
    /// </summary>
    /// <param name="languageRange">
    /// BCP-47 language range to filter results (e.g. <c>en</c>, <c>es</c>, <c>*</c>).
    /// Defaults to <c>en</c>.
    /// </param>
    /// <param name="pageToken">
    /// Opaque token from a previous response's <see cref="PagedResult{T}.NextPageToken"/>
    /// to retrieve the next page. Pass <see langword="null"/> for the first page.
    /// </param>
    /// <param name="pageSize">
    /// Maximum number of items to return. Pass <see langword="null"/> to use the API default.
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paged result containing <see cref="BibleVersionSummary"/> items.</returns>
    Task<PagedResult<BibleVersionSummary>> GetVersionsAsync(
        string languageRange = "en",
        string? pageToken = null,
        int? pageSize = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns full metadata for a single Bible version, including its list of available books.
    /// </summary>
    /// <param name="versionId">The numeric Bible version id (e.g. <c>3034</c> for BSB).</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="BibleVersion"/> metadata.</returns>
    Task<BibleVersion> GetVersionAsync(int versionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the books available in a given Bible version.
    /// </summary>
    /// <param name="versionId">The numeric Bible version id.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>
    /// A read-only list of <see cref="Book"/> records derived from the version metadata.
    /// </returns>
    Task<IReadOnlyList<Book>> GetBooksAsync(int versionId, CancellationToken cancellationToken = default);
}
