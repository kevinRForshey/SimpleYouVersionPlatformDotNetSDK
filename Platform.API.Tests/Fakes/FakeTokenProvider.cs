using System.Threading;
using System.Threading.Tasks;
using Platform.API.OAuth;

namespace Platform.API.Tests.Fakes;

/// <summary>
/// A controllable in-memory <see cref="ITokenProvider"/> for use in tests.
/// </summary>
internal sealed class FakeTokenProvider : ITokenProvider
{
    private OAuthTokenResponse? _token;

    public FakeTokenProvider(OAuthTokenResponse? initial = null)
    {
        _token = initial;
    }

    public Task<OAuthTokenResponse?> GetTokenAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(_token);

    public Task StoreTokenAsync(OAuthTokenResponse token, CancellationToken cancellationToken = default)
    {
        _token = token;
        return Task.CompletedTask;
    }

    public Task ClearTokenAsync(CancellationToken cancellationToken = default)
    {
        _token = null;
        return Task.CompletedTask;
    }
}
