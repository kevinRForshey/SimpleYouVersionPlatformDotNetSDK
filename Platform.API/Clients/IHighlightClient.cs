using System.Threading;
using System.Threading.Tasks;
using Platform.API.Models;

namespace Platform.API.Clients;

/// <summary>
/// Provides create, read, and delete operations for Bible verse highlights.
/// </summary>
/// <remarks>
/// Highlight operations require user authentication (OAuth). This interface is provided for
/// forward compatibility; the current implementation throws <see cref="System.NotSupportedException"/>
/// until OAuth support is added to the SDK.
/// </remarks>
public interface IHighlightClient
{
    /// <summary>
    /// Returns a paginated list of highlights for the authenticated user.
    /// </summary>
    /// <param name="pageToken">Opaque continuation token from a previous response, or <see langword="null"/> for the first page.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paged result of <see cref="Highlight"/> items.</returns>
    Task<PagedResult<Highlight>> GetHighlightsAsync(
        string? pageToken = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new highlight for a Bible verse.
    /// </summary>
    /// <param name="versionId">The numeric Bible version id.</param>
    /// <param name="usfm">The USFM verse identifier (e.g. <c>JHN.3.16</c>).</param>
    /// <param name="color">The highlight color to apply.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly created <see cref="Highlight"/>.</returns>
    Task<Highlight> CreateHighlightAsync(
        int versionId,
        string usfm,
        HighlightColor color,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an existing highlight by its id.
    /// </summary>
    /// <param name="highlightId">The unique identifier of the highlight to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    Task DeleteHighlightAsync(string highlightId, CancellationToken cancellationToken = default);
}
