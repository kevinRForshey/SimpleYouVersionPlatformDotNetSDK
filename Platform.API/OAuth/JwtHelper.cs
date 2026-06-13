using System.Text;
using System.Text.Json;

namespace Platform.API.OAuth;

/// <summary>
/// Lightweight helpers for extracting claims from an unsigned or signed JWT payload.
/// Only the Base64Url-encoded payload segment is read; signature verification is
/// intentionally omitted because these tokens are consumed from a trusted HTTPS response.
/// </summary>
internal static class JwtHelper
{
    /// <summary>
    /// Returns the string value of <paramref name="claimName"/> from the JWT payload,
    /// or <see langword="null"/> if the token is absent, malformed, or the claim is missing.
    /// </summary>
    internal static string? GetStringClaim(string? jwt, string claimName)
    {
        if (!TryDecodePayload(jwt, out var doc)) return null;
        using (doc)
        {
            return doc!.RootElement.TryGetProperty(claimName, out var val)
                ? val.GetString()
                : null;
        }
    }

    /// <summary>
    /// Returns the numeric (Unix-seconds) value of <paramref name="claimName"/> from
    /// the JWT payload, or <see langword="null"/> if absent or unparseable.
    /// </summary>
    internal static long? GetLongClaim(string? jwt, string claimName)
    {
        if (!TryDecodePayload(jwt, out var doc)) return null;
        using (doc)
        {
            if (!doc!.RootElement.TryGetProperty(claimName, out var val))
                return null;

            return val.ValueKind switch
            {
                JsonValueKind.Number when val.TryGetInt64(out var n) => n,
                JsonValueKind.String when long.TryParse(val.GetString(), out var n) => n,
                _ => null
            };
        }
    }

    // -------------------------------------------------------------------------
    // Private helpers
    // -------------------------------------------------------------------------

    private static bool TryDecodePayload(string? jwt, out JsonDocument? doc)
    {
        doc = null;
        if (string.IsNullOrEmpty(jwt)) return false;

        var parts = jwt.Split('.');
        if (parts.Length < 2) return false;

        var padded = parts[1].Replace('-', '+').Replace('_', '/');
        padded += new string('=', (4 - padded.Length % 4) % 4);

        try
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(padded));
            doc = JsonDocument.Parse(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
