using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Platform.API.Tests.Fakes;

/// <summary>
/// A test double for <see cref="HttpMessageHandler"/> that returns a configurable response.
/// </summary>
internal sealed class FakeHttpMessageHandler : HttpMessageHandler
{
    private readonly HttpResponseMessage _response;

    public FakeHttpMessageHandler(HttpStatusCode statusCode, string jsonBody)
    {
        _response = new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(jsonBody, Encoding.UTF8, "application/json")
        };
    }

    /// <summary>The last request received by this handler.</summary>
    public HttpRequestMessage? LastRequest { get; private set; }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(_response);
    }

    protected override void Dispose(bool disposing)
    {
        // Do not dispose _response here; callers hold it through the lifetime of the test.
    }
}
