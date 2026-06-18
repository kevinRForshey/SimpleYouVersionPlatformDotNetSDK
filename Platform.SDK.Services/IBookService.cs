using Platform.API.Models;

namespace Platform.SDK.Services
{
    public interface IBookService
    {
        Task<IReadOnlyList<Book>> GetBooksAsync(
            int versionId,
            CancellationToken cancellationToken = default);
    }
}
