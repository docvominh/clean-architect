using System.Net;
using System.Text;

using CleanArchitect.Infrastructure.ExternalSystem;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.ExternalSystem;

public sealed class FrankfurterExchangeRateClientTests
{
    private static IHttpClientFactory CreateHttpClientFactory(HttpMessageHandler? handler = null) =>
        new SingleClientFactory(new HttpClient(handler ?? new HttpClientHandler()) { BaseAddress = new Uri("https://api.frankfurter.dev/v2/") });

    [Fact]
    public async Task GetLatestRatesAsync_UsdToGbp_ReturnsPositiveRateForSymbol()
    {
        // Arrange
        IFrankfurterExchangeRateClient client = new FrankfurterExchangeRateClient(CreateHttpClientFactory());

        // Act
        var rate = await client.GetExchangeRateAsync("USD", "GBP");

        // Assert
        rate.Symbol.ShouldBe("GBP");
        rate.Rate.ShouldBeGreaterThan(0m);
    }

    [Fact]
    public async Task GetLatestRatesAsync_UnknownCurrencySymbol_Throws()
    {
        // Arrange
        IFrankfurterExchangeRateClient client = new FrankfurterExchangeRateClient(CreateHttpClientFactory());

        // Act
        var exception = await Record.ExceptionAsync(() =>
            client.GetExchangeRateAsync("USD", "ZZZ"));

        // Assert
        exception.ShouldNotBeNull();
    }

    [Fact]
    public async Task GetLatestRatesAsync_CalledTwice_RequestsFrankfurterEachTime()
    {
        // Arrange
        var handler = new CountingHandler();
        IFrankfurterExchangeRateClient client = new FrankfurterExchangeRateClient(CreateHttpClientFactory(handler));

        // Act
        var first = await client.GetExchangeRateAsync("USD", "GBP");
        var second = await client.GetExchangeRateAsync("USD", "GBP");

        // Assert
        handler.RequestCount.ShouldBe(2);
        second.ShouldBe(first with { Time = second.Time });
    }

    private sealed class SingleClientFactory(HttpClient httpClient) : IHttpClientFactory
    {
        public HttpClient CreateClient(string name) => httpClient;
    }

    private sealed class CountingHandler : HttpMessageHandler
    {
        public int RequestCount { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestCount++;
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("""{"date":"2026-09-15","base":"USD","quote":"GBP","rate":0.79}""", Encoding.UTF8, "application/json"),
            };
            return Task.FromResult(response);
        }
    }
}
