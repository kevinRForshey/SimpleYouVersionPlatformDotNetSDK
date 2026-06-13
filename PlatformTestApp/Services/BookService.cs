using Platform.API.Clients;
using Platform.API.Models;

namespace PlatformTestApp.Services;

public sealed class BookService(IBibleClient client) : IBookService
{
    public Task<IReadOnlyList<Book>> GetBooksAsync(
        int versionId,
        CancellationToken cancellationToken = default)
        => client.GetBooksAsync(versionId, cancellationToken);
}
