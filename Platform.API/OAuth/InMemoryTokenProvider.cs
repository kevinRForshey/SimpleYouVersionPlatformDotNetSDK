using System.Threading;
using System.Threading.Tasks;

namespace Platform.API.OAuth;

/// <summary>
/// A simple in-process, non-persistent <see cref="ITokenProvider"/> suitable for
/// console applications, background services, and tests.
/// </summary>
/// <remarks>
/// Tokens are stored in a private field and lost when the process exits.
/// Use a custom <see cref="ITokenProvider"/> implementation for scenarios that require
/// durable token storage (mobile, web, or desktop applications).
/// </remarks>
public sealed class InMemoryTokenProvider : ITokenProvider
{
    private OAuthTokenResponse? _token;

    /// <inheritdoc />
    public Task<OAuthTokenResponse?> GetTokenAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_token);

    /// <inheritdoc />
    public Task StoreTokenAsync(OAuthTokenResponse token, CancellationToken cancellationToken = default)
    {
        _token = token;
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ClearTokenAsync(CancellationToken cancellationToken = default)
    {
        _token = null;
        return Task.CompletedTask;
    }
}
