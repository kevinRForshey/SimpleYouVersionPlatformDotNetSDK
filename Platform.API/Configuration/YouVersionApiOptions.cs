using System;
using System.ComponentModel.DataAnnotations;

namespace Platform.API.Configuration;

/// <summary>
/// Configuration options for the YouVersion Platform API client.
/// Bind to the <c>YouVersionApi</c> configuration section or supply values directly.
/// </summary>
public sealed class YouVersionApiOptions
{
    /// <summary>
    /// The configuration section name to bind from <c>appsettings.json</c> or environment variables.
    /// </summary>
    public const string SectionName = "YouVersionApi";

    /// <summary>
    /// Your YouVersion Platform app key. Sent as the <c>X-YVP-App-Key</c> header on every request.
    /// App keys are <em>not</em> secrets and may be included in source code or client bundles.
    /// Obtain one at <see href="https://platform.youversion.com"/>.
    /// </summary>
    [Required]
    public string AppKey { get; set; } = string.Empty;

    /// <summary>
    /// The base URL of the YouVersion Platform API.
    /// Defaults to <c>https://api.youversion.com</c>.
    /// Override only in testing scenarios.
    /// </summary>
    public Uri BaseAddress { get; set; } = new("https://api.youversion.com");

    /// <summary>
    /// HTTP request timeout. Defaults to 30 seconds.
    /// </summary>
    public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// How many seconds before a token's actual expiry to proactively refresh it inside
    /// <see cref="Platform.API.Http.OAuthBearerTokenHandler"/>.
    /// Defaults to 60 seconds.
    /// </summary>
    public int OAuthTokenExpiryBufferSeconds { get; set; } = 60;
}
