using System;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Platform.API.OAuth;

/// <summary>
/// Represents the token response returned by the YouVersion OAuth 2.0 token endpoint.
/// </summary>
public sealed record OAuthTokenResponse
{
    /// <summary>The access token to use in <c>Authorization: Bearer</c> headers.</summary>
    [JsonPropertyName("access_token")]
    public string AccessToken { get; init; } = string.Empty;

    /// <summary>The refresh token used to obtain a new access token without user interaction.</summary>
    [JsonPropertyName("refresh_token")]
    public string? RefreshToken { get; init; }

    /// <summary>OpenID Connect ID token containing user identity claims.</summary>
    [JsonPropertyName("id_token")]
    public string? IdToken { get; init; }

    /// <summary>The token type. Always <c>Bearer</c> for this API.</summary>
    [JsonPropertyName("token_type")]
    public string TokenType { get; init; } = "Bearer";

    /// <summary>The lifetime of the access token in seconds.</summary>
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// The UTC time at which this response was received.
    /// Used to calculate whether the token has expired.
    /// </summary>
    [JsonIgnore]
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Returns <see langword="true"/> if the access token is expired or within
    /// <paramref name="bufferSeconds"/> of expiring.
    /// </summary>
    public bool IsExpired(int bufferSeconds = 60) =>
        DateTimeOffset.UtcNow >= ReceivedAt.AddSeconds(ExpiresIn - bufferSeconds);

    /// <summary>
    /// Decodes the <see cref="IdToken"/> JWT payload and returns the value of the
    /// specified claim, or <see langword="null"/> if the token is absent or the claim
    /// is not present.
    /// </summary>
    public string? GetClaim(string claimName) => GetClaimFromJwt(IdToken, claimName);

    /// <summary>
    /// Returns the user's display name by searching common OIDC claim names
    /// (<c>name</c>, <c>preferred_username</c>) in the ID token first, then
    /// falling back to the same claims in the access token (which is often a JWT
    /// that contains user identity even when no <c>id_token</c> is issued).
    /// </summary>
    public string? GetUserName() =>
        GetClaimFromJwt(IdToken, "name") ??
        GetClaimFromJwt(IdToken, "preferred_username") ??
        GetClaimFromJwt(AccessToken, "name") ??
        GetClaimFromJwt(AccessToken, "preferred_username");

    /// <summary>
    /// Decodes the Base64Url-encoded payload of <paramref name="jwt"/> and returns
    /// the string value of <paramref name="claimName"/>, or <see langword="null"/>
    /// if the JWT is absent, malformed, or the claim is not present.
    /// </summary>
    private static string? GetClaimFromJwt(string? jwt, string claimName)
    {
        if (string.IsNullOrEmpty(jwt)) return null;
        var parts = jwt.Split('.');
        if (parts.Length < 2) return null;

        var padded = parts[1].Replace('-', '+').Replace('_', '/');
        padded += new string('=', (4 - padded.Length % 4) % 4);

        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty(claimName, out var val) ? val.GetString() : null;
        }
        catch
        {
            return null;
        }
    }
}
