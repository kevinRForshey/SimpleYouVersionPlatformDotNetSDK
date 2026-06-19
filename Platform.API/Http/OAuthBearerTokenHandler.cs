using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Platform.API.OAuth;

namespace Platform.API.Http;

/// <summary>
/// A <see cref="DelegatingHandler"/> that retrieves the current OAuth access token from
/// <see cref="ITokenProvider"/> and attaches it as an
/// <c>Authorization: Bearer &lt;token&gt;</c> header.
/// If the token is expired and a refresh token exists, it is refreshed automatically.
/// </summary>
internal sealed class OAuthBearerTokenHandler : DelegatingHandler
{
    private readonly ITokenProvider _tokenProvider;
    private readonly IYouVersionOAuthClient _oAuthClient;
    private readonly YouVersionOAuthOptions _oAuthOptions;
    private readonly ILogger<OAuthBearerTokenHandler> _logger;

    public OAuthBearerTokenHandler(
        ITokenProvider tokenProvider,
        IYouVersionOAuthClient oAuthClient,
        IOptions<YouVersionOAuthOptions> oAuthOptions,
        ILogger<OAuthBearerTokenHandler> logger)
    {
        _tokenProvider = tokenProvider;
        _oAuthClient = oAuthClient;
        _oAuthOptions = oAuthOptions.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var token = await _tokenProvider.GetTokenAsync(cancellationToken).ConfigureAwait(false);

        if (token is not null && token.IsExpired(_oAuthOptions.OAuthTokenExpiryBufferSeconds))
        {
            _logger.LogDebug("OAuth access token expired; attempting transparent refresh.");
            try
            {
                token = await _oAuthClient.RefreshTokenAsync(cancellationToken).ConfigureAwait(false);
            }
            catch (System.InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Token refresh unavailable; proceeding without bearer token.");
                token = null;
            }
        }

        if (token is not null)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);
        }
        else
        {
            _logger.LogDebug("No OAuth token stored; request will proceed unauthenticated.");
        }

        return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
    }
}
