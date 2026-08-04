using FluentAssertions;
using Moq;
using Platform.API.OAuth;
using Xunit;

namespace Platform.SDK.Services.Tests;

public sealed class AuthSessionServiceTests
{
    private static OAuthTokenResponse MakeToken(int expiresInSeconds, DateTimeOffset receivedAt) => new()
    {
        AccessToken = "access-token",
        ExpiresIn = expiresInSeconds,
        ReceivedAt = receivedAt,
    };

    [Fact]
    public async Task GetCurrentSessionAsync_WhenNoTokenStored_ReturnsSignedOut()
    {
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider.Setup(t => t.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync((OAuthTokenResponse?)null);

        var sut = new AuthSessionService(tokenProvider.Object);

        var session = await sut.GetCurrentSessionAsync();

        session.Should().Be(AuthSession.SignedOut);
    }

    [Fact]
    public async Task GetCurrentSessionAsync_WhenTokenExpired_ReturnsSignedOut()
    {
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider
            .Setup(t => t.GetTokenAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(MakeToken(60, DateTimeOffset.UtcNow.AddHours(-1)));

        var sut = new AuthSessionService(tokenProvider.Object);

        var session = await sut.GetCurrentSessionAsync();

        session.Should().Be(AuthSession.SignedOut);
    }

    [Fact]
    public async Task GetCurrentSessionAsync_WhenTokenValid_ReturnsSignedInWithDisplayIdentity()
    {
        var claims = new Dictionary<string, string> { ["name"] = "Jane Doe" };
        var payloadJson = System.Text.Json.JsonSerializer.Serialize(claims);
        var payload = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(payloadJson));
        var idToken = $"header.{payload}.signature";

        var token = new OAuthTokenResponse
        {
            AccessToken = "access-token",
            IdToken = idToken,
            ExpiresIn = 3600,
            ReceivedAt = DateTimeOffset.UtcNow
        };
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider.Setup(t => t.GetTokenAsync(It.IsAny<CancellationToken>())).ReturnsAsync(token);

        var sut = new AuthSessionService(tokenProvider.Object);

        var session = await sut.GetCurrentSessionAsync();

        session.IsSignedIn.Should().BeTrue();
        session.DisplayName.Should().Be("Jane Doe");
    }

    [Fact]
    public async Task GetCurrentSessionAsync_PropagatesCancellationTokenToTokenProvider()
    {
        using var cts = new CancellationTokenSource();
        var tokenProvider = new Mock<ITokenProvider>();
        tokenProvider.Setup(t => t.GetTokenAsync(cts.Token)).ReturnsAsync((OAuthTokenResponse?)null);

        var sut = new AuthSessionService(tokenProvider.Object);
        await sut.GetCurrentSessionAsync(cts.Token);

        tokenProvider.Verify(t => t.GetTokenAsync(cts.Token), Times.Once);
    }
}
