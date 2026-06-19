using Platform.API.Models;

namespace Platform.API.Clients;

/// <summary>
/// Write surface of the highlights API. Requires OAuth bearer-token authentication.
/// </summary>
/// <remarks>
/// Consumers that only need to create or delete highlights should depend on this interface.
/// Register <see cref="IHighlightClient"/> in the DI container; it implements both
/// <see cref="IHighlightReader"/> and <see cref="IHighlightWriter"/>.
/// </remarks>
public interface IHighlightWriter
{
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
