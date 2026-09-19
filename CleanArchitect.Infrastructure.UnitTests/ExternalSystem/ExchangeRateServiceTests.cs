using CleanArchitect.Application.ExternalSystem;
using CleanArchitect.Infrastructure.ExternalSystem;

using Microsoft.Extensions.Caching.Memory;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Infrastructure.UnitTests.ExternalSystem;

public sealed class ExchangeRateServiceTests
{
    private readonly Mock<IFrankfurterExchangeRateClient> frankfurterClient = new();
    private readonly IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetLatestRatesAsync_FirstCall_FetchesFromClient()
    {
        // Arrange
        var rate = new CurrencyRate("GBP", 0.79m, DateTimeOffset.UtcNow);
        frankfurterClient.Setup(c => c.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(rate);
        var service = new ExchangeRateService(frankfurterClient.Object, memoryCache);

        // Act
        var result = await service.GetLatestRatesAsync("USD", "GBP");

        // Assert
        result.ShouldBe(rate);
        frankfurterClient.Verify(c => c.GetExchangeRateAsync("USD", "GBP"), Times.Once);
    }

    [Fact]
    public async Task GetLatestRatesAsync_CalledTwiceForSamePair_OnlyFetchesFromClientOnce()
    {
        // Arrange
        var rate = new CurrencyRate("GBP", 0.79m, DateTimeOffset.UtcNow);
        frankfurterClient.Setup(c => c.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(rate);
        var service = new ExchangeRateService(frankfurterClient.Object, memoryCache);

        // Act
        var first = await service.GetLatestRatesAsync("USD", "GBP");
        var second = await service.GetLatestRatesAsync("USD", "GBP");

        // Assert
        second.ShouldBe(first);
        frankfurterClient.Verify(c => c.GetExchangeRateAsync("USD", "GBP"), Times.Once);
    }

    [Fact]
    public async Task GetLatestRatesAsync_CalledForDifferentCurrencyPair_FetchesFromClientAgain()
    {
        // Arrange
        var usdGbp = new CurrencyRate("GBP", 0.79m, DateTimeOffset.UtcNow);
        var usdEur = new CurrencyRate("EUR", 0.92m, DateTimeOffset.UtcNow);
        frankfurterClient.Setup(c => c.GetExchangeRateAsync("USD", "GBP")).ReturnsAsync(usdGbp);
        frankfurterClient.Setup(c => c.GetExchangeRateAsync("USD", "EUR")).ReturnsAsync(usdEur);
        var service = new ExchangeRateService(frankfurterClient.Object, memoryCache);

        // Act
        var gbpResult = await service.GetLatestRatesAsync("USD", "GBP");
        var eurResult = await service.GetLatestRatesAsync("USD", "EUR");

        // Assert
        gbpResult.ShouldBe(usdGbp);
        eurResult.ShouldBe(usdEur);
        frankfurterClient.Verify(c => c.GetExchangeRateAsync("USD", "GBP"), Times.Once);
        frankfurterClient.Verify(c => c.GetExchangeRateAsync("USD", "EUR"), Times.Once);
    }

    [Fact]
    public async Task GetLatestRatesAsync_ClientThrows_PropagatesExceptionAndDoesNotCacheFailure()
    {
        // Arrange
        frankfurterClient.SetupSequence(c => c.GetExchangeRateAsync("USD", "ZZZ"))
            .ThrowsAsync(new InvalidOperationException("Frankfurter returned no exchange rates."))
            .ReturnsAsync(new CurrencyRate("ZZZ", 1.23m, DateTimeOffset.UtcNow));
        var service = new ExchangeRateService(frankfurterClient.Object, memoryCache);

        // Act
        async Task Action()
        {
            await service.GetLatestRatesAsync("USD", "ZZZ");
        }

        // Assert
        await Should.ThrowAsync<InvalidOperationException>((Func<Task>)Action);
        var retryResult = await service.GetLatestRatesAsync("USD", "ZZZ");
        retryResult.Rate.ShouldBe(1.23m);
        frankfurterClient.Verify(c => c.GetExchangeRateAsync("USD", "ZZZ"), Times.Exactly(2));
    }
}
