using Platform.API.Clients;
using Platform.API.Models;

namespace PlatformTestApp.Services;

public class PassageService(IPassageClient client)
{
    public async Task<Passage> GetPassageAsync(
        int versionId,
        string usfm,
        PassageRequestOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return await client.GetPassageAsync(versionId, usfm, options, cancellationToken);
    }
}
