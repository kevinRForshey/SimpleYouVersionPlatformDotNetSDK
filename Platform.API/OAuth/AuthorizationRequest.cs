namespace Platform.API.OAuth;

/// <summary>
/// Holds the authorization URL and the matching PKCE values produced by
/// <see cref="IYouVersionOAuthClient.BuildAuthorizationUrl"/>.
/// </summary>
/// <remarks>
/// Store <see cref="Pkce"/>.<see cref="PkceValues.CodeVerifier"/> in server-side
/// session state and present it to <see cref="IYouVersionOAuthClient.ExchangeCodeAsync"/>
/// when the authorization server redirects the user back with a code.
/// </remarks>
public sealed record AuthorizationRequest
{
    /// <summary>The fully-formed URL to redirect the user to for sign-in.</summary>
    public Uri AuthorizationUrl { get; init; } = null!;

    /// <summary>
    /// The PKCE values generated for this request. Keep
    /// <see cref="PkceValues.CodeVerifier"/> secret and server-side.
    /// </summary>
    public PkceValues Pkce { get; init; } = new();
}
