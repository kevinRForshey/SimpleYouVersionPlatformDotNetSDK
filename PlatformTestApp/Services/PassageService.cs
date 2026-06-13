using Platform.API.Clients;
using Platform.API.Models;

namespace PlatformTestApp.Services;

public sealed class PassageService(IPassageClient client)
{
    public Task<Passage> GetPassageAsync(
        int versionId,
        string usfm,
        PassageRequestOptions? options = null,
        CancellationToken cancellationToken = default)
        => client.GetPassageAsync(versionId, usfm, options, cancellationToken);
}
