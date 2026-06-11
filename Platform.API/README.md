# YouVersion.Platform.API

Typed HTTP client SDK for the [YouVersion Platform REST API](https://developers.youversion.com).

## Features

- `IBibleClient` — discover Bible versions and books
- `IPassageClient` — fetch scripture in plain text or HTML
- `IHighlightClient` — create, list, and delete verse highlights (requires OAuth)
- `IYouVersionOAuthClient` — full authorization-code + PKCE flow
- `ITokenProvider` / `InMemoryTokenProvider` — pluggable token storage
- `Microsoft.Extensions.DependencyInjection` integration via `AddYouVersionApiClients()` / `AddYouVersionOAuth()`
- Structured `ILogger<T>` logging on every client

## Quick start

```csharp
// Program.cs / Startup.cs
builder.Services
    .AddYouVersionApiClients(o => o.AppKey = "YOUR_APP_KEY")
    .AddYouVersionOAuth(o =>
    {
        o.ClientId    = "YOUR_OAUTH_CLIENT_ID";
        o.RedirectUri = new Uri("https://yourapp.com/oauth/callback");
    });
```

Then inject whichever interface you need:

```csharp
public class BibleService(IBibleClient bible, IPassageClient passages)
{
    public async Task<string> GetJohn316Async()
    {
        var passage = await passages.GetPassageAsync(3034, "JHN.3.16");
        return passage.Content;
    }
}
```

## Target framework

`net10.0`

## Documentation

- [Getting started](../docs/getting-started.md)
- [Authentication (app key)](../docs/authentication.md)
- [OAuth guide](../docs/oauth-guide.md)

## NuGet packaging

```powershell
dotnet pack -c Release
```
