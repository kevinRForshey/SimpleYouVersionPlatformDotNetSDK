using Platform.API.Clients;
using Platform.API.Models;

namespace Platform.SDK.Services
{
    public sealed class BookService(IBibleClient client) : IBookService
    {
        public Task<IReadOnlyList<Book>> GetBooksAsync(
            int versionId,
            CancellationToken cancellationToken = default)
            => client.GetBooksAsync(versionId, cancellationToken);
    }
}
