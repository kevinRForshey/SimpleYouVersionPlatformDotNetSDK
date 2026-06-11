using Platform.API.Models;

namespace PlatformTestApp.Services;

public interface IVersionService
{
    Task<IReadOnlyList<BibleVersionSummary>> GetVersionsAsync(
        string languageRange = "en",
        CancellationToken cancellationToken = default);
}
