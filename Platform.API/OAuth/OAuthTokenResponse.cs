using System;
using System.Text.Json.Serialization;

namespace Platform.API.OAuth;

/// <summary>
/// Represents the token response returned by the platform's OAuth 2.0 token endpoint.
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
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
    public int ExpiresIn { get; init; }

    /// <summary>
    /// The UTC time at which this response was received.
    /// Used to calculate whether the token has expired.
    /// </summary>
    /// <remarks>
    /// This property is excluded from JSON serialization. If you persist the token
    /// (e.g. in a cache or on disk), you must separately store and restore
    /// <see cref="ReceivedAt"/>; otherwise it resets to the current time on
    /// deserialization, making a near-expired token appear fresh.
    /// </remarks>
    [JsonIgnore]
    public DateTimeOffset ReceivedAt { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Returns <see langword="true"/> if the access token is expired or within
    /// <paramref name="bufferSeconds"/> of expiring.
    /// </summary>
    public bool IsExpired(int bufferSeconds = 60)
    {
        if (string.IsNullOrWhiteSpace(AccessToken))
            return true;

        if (ExpiresIn > 0)
            return DateTimeOffset.UtcNow >= ReceivedAt.AddSeconds(ExpiresIn - bufferSeconds);

        var exp = JwtHelper.GetLongClaim(IdToken, "exp") ?? JwtHelper.GetLongClaim(AccessToken, "exp");
        if (exp is long unixSeconds)
            return DateTimeOffset.UtcNow >= DateTimeOffset.FromUnixTimeSeconds(unixSeconds).AddSeconds(-bufferSeconds);

        // Some providers omit explicit expiration in exchange responses.
        return false;
    }

    /// <summary>User display name returned directly in top-level token response JSON, if any.</summary>
    [JsonPropertyName("user_name")]
    public string? UserName { get; init; }

    /// <summary>User display name returned directly in top-level token response JSON, if any.</summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>User preferred username returned directly in top-level token response JSON, if any.</summary>
    [JsonPropertyName("preferred_username")]
    public string? PreferredUsername { get; init; }

    /// <summary>User email returned directly in top-level token response JSON, if any.</summary>
    [JsonPropertyName("user_email")]
    public string? UserEmail { get; init; }

    /// <summary>User email returned directly in top-level token response JSON, if any.</summary>
    [JsonPropertyName("email")]
    public string? Email { get; init; }

    /// <summary>Platform user id returned directly in top-level token response JSON, if any.</summary>
    [JsonPropertyName("yvp_id")]
    public string? YvpId { get; init; }

    /// <summary>
    /// Decodes the <see cref="IdToken"/> JWT payload and returns the value of the
    /// specified claim, or <see langword="null"/> if the token is absent or the claim
    /// is not present.
    /// </summary>
    public string? GetClaim(string claimName) =>
        JwtHelper.GetStringClaim(IdToken, claimName) ?? JwtHelper.GetStringClaim(AccessToken, claimName);

    /// <summary>
    /// Returns the user's display name by searching common OIDC claim names
    /// in top-level properties, the ID token, and the access token.
    /// </summary>
    public string? GetUserName() =>
        (!string.IsNullOrWhiteSpace(Name) ? Name : null) ??
        (!string.IsNullOrWhiteSpace(UserName) ? UserName : null) ??
        (!string.IsNullOrWhiteSpace(PreferredUsername) ? PreferredUsername : null) ??
        JwtHelper.GetFirstAvailableClaim(IdToken, "name", "preferred_username", "user_name", "username", "display_name", "given_name", "family_name", "nickname", "unique_name", "user") ??
        JwtHelper.GetFirstAvailableClaim(AccessToken, "name", "preferred_username", "user_name", "username", "display_name", "given_name", "family_name", "nickname", "unique_name", "user");

    /// <summary>
    /// Returns the user's email address by searching common claim names
    /// in top-level properties, the ID token, and the access token.
    /// </summary>
    public string? GetEmail() =>
        (!string.IsNullOrWhiteSpace(Email) ? Email : null) ??
        (!string.IsNullOrWhiteSpace(UserEmail) ? UserEmail : null) ??
        JwtHelper.GetFirstAvailableClaim(IdToken, "email", "user_email", "email_address", "upn", "mail") ??
        JwtHelper.GetFirstAvailableClaim(AccessToken, "email", "user_email", "email_address", "upn", "mail");

    /// <summary>
    /// Returns the best available user identifier for display in the UI.
    /// Prefers name, then email, then platform-specific stable identifiers.
    /// </summary>
    public string? GetDisplayIdentity() =>
        GetUserName() ??
        GetEmail() ??
        (!string.IsNullOrWhiteSpace(YvpId) ? YvpId : null) ??
        JwtHelper.GetFirstAvailableClaim(IdToken, "yvp_id", "sub", "user_id", "id") ??
        JwtHelper.GetFirstAvailableClaim(AccessToken, "yvp_id", "sub", "user_id", "id");
}
