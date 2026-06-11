using System;

namespace Platform.API.OAuth;

/// <summary>
/// Configuration options for YouVersion OAuth 2.0 with PKCE.
/// Bind to the <c>YouVersionOAuth</c> configuration section or configure inline.
/// </summary>
public sealed class YouVersionOAuthOptions
{
    /// <summary>The configuration section name used when binding from <c>IConfiguration</c>.</summary>
    public const string SectionName = "YouVersionOAuth";

    /// <summary>
    /// The OAuth 2.0 client identifier registered in the YouVersion developer portal.
    /// </summary>
    public string ClientId { get; set; } = string.Empty;

    /// <summary>
    /// The URI the authorization server redirects to after the user grants or denies access.
    /// Must match a URI registered in the YouVersion developer portal.
    /// </summary>
    public Uri? RedirectUri { get; set; }

    /// <summary>
    /// The OAuth 2.0 authorization endpoint URL.
    /// Defaults to the YouVersion authorization server.
    /// </summary>
    public Uri AuthorizationEndpoint { get; set; } =
        new("https://api.youversion.com/auth/authorize");

    /// <summary>
    /// The OAuth 2.0 token endpoint URL.
    /// Defaults to the YouVersion token server.
    /// </summary>
    public Uri TokenEndpoint { get; set; } =
        new("https://api.youversion.com/auth/token");

    /// <summary>
    /// Space-separated OAuth scopes to request.
    /// <list type="bullet">
    ///   <item><description><c>passages</c> — read Bible text on behalf of the user.</description></item>
    ///   <item><description><c>highlights</c> — read and write the user's verse highlights.</description></item>
    /// </list>
    /// Bible version discovery and public passage reads use the app key only and do not
    /// require an OAuth token or scope.
    /// </summary>
    public string Scopes { get; set; } = "passages highlights";
}
