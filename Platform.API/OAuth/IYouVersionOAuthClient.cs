using Platform.API.Clients;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.API.OAuth;

/// <summary>
/// Provides YouVersion OAuth 2.0 authorization-code + PKCE flow operations.
/// </summary>
/// <remarks>
/// Typical usage:
/// <list type="number">
///   <item><description>Call <see cref="BuildAuthorizationUrl"/> to get the URL to redirect the user to.</description></item>
///   <item><description>The user authenticates and is redirected back with a <c>code</c> query parameter.</description></item>
///   <item><description>Call <see cref="ExchangeCodeAsync"/> with that code. The resulting token is automatically stored via <see cref="ITokenProvider"/>.</description></item>
///   <item><description>Subsequent API calls with <see cref="Clients.IHighlightClient"/> are now authorized automatically.</description></item>
///   <item><description>Call <see cref="RefreshTokenAsync"/> proactively, or rely on <c>OAuthBearerTokenHandler</c> to refresh transparently.</description></item>
///   <item><description>Call <see cref="SignOutAsync"/> to clear the stored token on sign-out.</description></item>
/// </list>
/// </remarks>
public interface IYouVersionOAuthClient
{
    /// <summary>
    /// Builds the authorization URL to redirect the user to for sign-in.
    /// </summary>
    /// <param name="pkce">
    /// Output: the generated <see cref="PkceValues"/> containing the code verifier
    /// and challenge. Store <see cref="PkceValues.CodeVerifier"/> securely (e.g., session state)
    /// for use in <see cref="ExchangeCodeAsync"/>.
    /// </param>
    /// <param name="state">
    /// Optional opaque state string for CSRF protection. If omitted, a random value is generated.
    /// Verify this value matches the <c>state</c> parameter on the callback.
    /// </param>
    /// <returns>The fully-formed authorization URL to redirect the user to.</returns>
    Uri BuildAuthorizationUrl(out PkceValues pkce, string? state = null);

    /// <summary>
    /// Exchanges an authorization code for access and refresh tokens.
    /// The resulting token is stored via <see cref="ITokenProvider"/>.
    /// </summary>
    /// <param name="code">The authorization code received in the redirect callback.</param>
    /// <param name="codeVerifier">The PKCE code verifier from the matching <see cref="PkceValues"/>.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="OAuthTokenResponse"/> containing the access and refresh tokens.</returns>
    Task<OAuthTokenResponse> ExchangeCodeAsync(
        string code,
        string codeVerifier,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Uses the stored refresh token to obtain a new access token.
    /// The new token is stored via <see cref="ITokenProvider"/>.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The refreshed <see cref="OAuthTokenResponse"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no refresh token is available.</exception>
    Task<OAuthTokenResponse> RefreshTokenAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Clears the stored token. Call this on user sign-out.
    /// </summary>
    Task SignOutAsync(CancellationToken cancellationToken = default);
}
