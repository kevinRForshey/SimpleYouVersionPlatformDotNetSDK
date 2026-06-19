namespace Platform.API.OAuth;

/// <summary>
/// Holds the PKCE code verifier and the derived code challenge for an authorization request.
/// </summary>
public sealed record PkceValues
{
    /// <summary>The high-entropy random code verifier (kept secret until token exchange).</summary>
    public string CodeVerifier { get; init; } = string.Empty;

    /// <summary>The SHA-256 Base64Url-encoded code challenge sent in the authorization URL.</summary>
    public string CodeChallenge { get; init; } = string.Empty;

    /// <summary>Always <c>S256</c>.</summary>
    public string CodeChallengeMethod { get; init; } = "S256";
}
