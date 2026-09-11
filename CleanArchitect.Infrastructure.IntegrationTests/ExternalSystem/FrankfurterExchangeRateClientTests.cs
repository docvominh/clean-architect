using CleanArchitect.Application.ExternalSystem;
using CleanArchitect.Infrastructure.ExternalSystem;

using Shouldly;

namespace CleanArchitect.Infrastructure.IntegrationTests.ExternalSystem;

public sealed class FrankfurterExchangeRateClientTests
{
    private static HttpClient CreateHttpClient() => new() { BaseAddress = new Uri("https://api.frankfurter.dev/v1/") };

    [Fact]
    public async Task GetLatestRatesAsync_UsdToGbpAndEur_ReturnsPositiveRatesForBothCurrencies()
    {
        // Arrange
        IExchangeRate client = new FrankfurterExchangeRateClient(CreateHttpClient());

        // Act
        var rates = await client.GetLatestRatesAsync("USD", ["GBP", "EUR"], CancellationToken.None);

        // Assert
        rates.Keys.ShouldBe(["GBP", "EUR"], ignoreOrder: true);
        rates["GBP"].ShouldBeGreaterThan(0m);
        rates["EUR"].ShouldBeGreaterThan(0m);
    }

    [Fact]
    public async Task GetLatestRatesAsync_UnknownCurrencySymbol_Throws()
    {
        // Arrange
        IExchangeRate client = new FrankfurterExchangeRateClient(CreateHttpClient());

        // Act
        var exception = await Record.ExceptionAsync(() =>
            client.GetLatestRatesAsync("USD", ["ZZZ"], CancellationToken.None));

        // Assert
        exception.ShouldNotBeNull();
    }
}
