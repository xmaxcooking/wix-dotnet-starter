namespace App.Updates.Tests;

/// <summary>Fails the test loudly if UpdateCheck ever reaches the download step when it shouldn't.</summary>
internal sealed class ThrowingHttpClientFactory : IHttpClientFactory
{
    public HttpClient CreateClient(string name) =>
        throw new InvalidOperationException("CreateClient should not have been called.");
}