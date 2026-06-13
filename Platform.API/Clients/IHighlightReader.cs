using Platform.API.Models;

namespace Platform.API.Clients;

/// <summary>
/// Read-only surface of the highlights API. Requires only app-key authentication.
/// </summary>
/// <remarks>
/// Consumers that only need to read highlights should depend on this interface
/// rather than <see cref="IHighlightClient"/> to minimise their surface area.
/// </remarks>
public interface IHighlightReader
{
    /// <summary>
    /// Returns a paginated list of highlights for the authenticated user.
    /// </summary>
    /// <param name="pageToken">Opaque continuation token, or <see langword="null"/> for the first page.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A paged result of <see cref="Highlight"/> items.</returns>
    Task<PagedResult<Highlight>> GetHighlightsAsync(
        string? pageToken = null,
        CancellationToken cancellationToken = default);
}
