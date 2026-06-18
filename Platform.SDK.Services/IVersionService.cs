using Platform.API.Models;

namespace Platform.SDK.Services
{
    public interface IVersionService
    {
        Task<IReadOnlyList<BibleVersionSummary>> GetVersionsAsync(
            string languageRange = "en",
            CancellationToken cancellationToken = default);
    }
}
