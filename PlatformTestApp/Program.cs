using System.Security.Cryptography;

using Platform.API.Extensions;
using Platform.API.OAuth;

using PlatformTestApp.Components;
using PlatformTestApp.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddYouVersionApiClients(builder.Configuration);
builder.Services.AddYouVersionOAuth(o => builder.Configuration.GetSection("YouVersionOAuth").Bind(o));

// App-layer services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IBibleReaderStateService, BibleReaderStateService>();
builder.Services.AddScoped<IVersionService, VersionService>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<PassageService>();

// Session support for OAuth PKCE code verifier / state storage
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(o =>
{
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
    o.IdleTimeout = TimeSpan.FromMinutes(15);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseSession();

// YouVersion redirects back to http://localhost:52413?code=...&state=...
// Park the code in the session then hand off to the callback endpoint which has a real HttpContext.
app.Use(async (ctx, next) =>
{
    if (ctx.Request.Path == "/" && ctx.Request.Query.ContainsKey("code"))
    {
        ctx.Session.SetString("oauth_code",         ctx.Request.Query["code"].ToString());
        ctx.Session.SetString("oauth_state_return", ctx.Request.Query["state"].ToString());
        ctx.Response.Redirect("/auth/callback-complete");
        return;
    }
    await next(ctx);
});

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// OAuth login redirect endpoint — writes PKCE verifier to session then redirects
// to the YouVersion authorization server. Must be a minimal API (not a Blazor page)
// so HttpContext.Session is writable before the external redirect occurs.
app.MapGet("/auth/login", (IYouVersionOAuthClient oauthClient, HttpContext ctx) =>
{
    var state = Base64Url(RandomNumberGenerator.GetBytes(16));
    var url = oauthClient.BuildAuthorizationUrl(out var pkce, state);
    ctx.Session.SetString("pkce_verifier", pkce.CodeVerifier);
    ctx.Session.SetString("oauth_state", state);
    return Results.Redirect(url.ToString());
});

app.MapGet("/auth/logout", async (IYouVersionOAuthClient oauthClient, HttpContext ctx) =>
{
    await oauthClient.SignOutAsync();
    ctx.Session.Clear();
    return Results.Redirect("/");
});

// Completes the OAuth code exchange. Called by the middleware after parking the code in session.
// Minimal API endpoints have a real HttpContext and reliable session access — Blazor components don't.
app.MapGet("/auth/callback-complete", async (IYouVersionOAuthClient oauthClient, HttpContext ctx) =>
{
    var code     = ctx.Session.GetString("oauth_code");
    var retState = ctx.Session.GetString("oauth_state_return");
    var verifier = ctx.Session.GetString("pkce_verifier");
    var expected = ctx.Session.GetString("oauth_state");

    string? exchangeError = null;

    if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(verifier))
    {
        exchangeError = "Session expired or invalid callback. Please try signing in again.";
    }
    else if (!string.IsNullOrEmpty(expected) && expected != retState)
    {
        exchangeError = "State mismatch — possible CSRF attempt. Please try signing in again.";
    }
    else
    {
        try
        {
            await oauthClient.ExchangeCodeAsync(code, verifier);
        }
        catch (Exception ex)
        {
            ctx.RequestServices.GetRequiredService<ILogger<Program>>()
                .LogError(ex, "OAuth code exchange failed.");
            exchangeError = "Sign-in failed. Please try again.";
        }
    }

    ctx.Session.Remove("oauth_code");
    ctx.Session.Remove("oauth_state_return");
    ctx.Session.Remove("pkce_verifier");
    ctx.Session.Remove("oauth_state");

    return exchangeError is not null
        ? Results.Redirect($"/?oauth_error={Uri.EscapeDataString(exchangeError)}")
        : Results.Redirect("/");
});

app.Run();

static string Base64Url(byte[] bytes) =>
    Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');
