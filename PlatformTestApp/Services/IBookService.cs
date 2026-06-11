using Platform.API.Models;

namespace PlatformTestApp.Services;

public interface IBookService
{
    Task<IReadOnlyList<Book>> GetBooksAsync(
        int versionId,
        CancellationToken cancellationToken = default);
}
