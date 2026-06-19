using System.Threading;
using System.Threading.Tasks;
using Platform.API.Models;

namespace Platform.API.Clients;

/// <summary>
/// Provides scripture content retrieval from the YouVersion Platform API.
/// </summary>
public interface IPassageClient
{
    /// <summary>
    /// Retrieves the content of a Bible passage.
    /// </summary>
    /// <remarks>
    /// Passage ranges must fall within a single chapter (e.g. <c>JHN.3.16-17</c>).
    /// For cross-chapter ranges, make multiple calls and combine the results.
    /// Always display <see cref="Passage.Reference"/> and the version's copyright alongside
    /// the returned content.
    /// </remarks>
    /// <param name="versionId">The numeric Bible version id (e.g. <c>3034</c> for BSB).</param>
    /// <param name="usfm">
    /// The USFM passage identifier (e.g. <c>JHN.3.16</c>, <c>GEN.1</c>, <c>MAT.1.1-5</c>).
    /// </param>
    /// <param name="options">
    /// Options controlling format, headings, and footnotes.
    /// Defaults to <see cref="PassageRequestOptions.Default"/> (plain text, no extras).
    /// </param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The requested <see cref="Passage"/>.</returns>
    Task<Passage> GetPassageAsync(
        int versionId,
        string usfm,
        PassageRequestOptions? options = null,
        CancellationToken cancellationToken = default);
}
