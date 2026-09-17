using CleanArchitect.Application.ExternalSystem;
using CleanArchitect.Application.ExternalSystem.Query;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ExternalSystem;

public class GetExchangeRatesQueryHandlerTests
{
    private readonly Mock<IExchangeRateService> exchangeRate = new();

    [Fact]
    public async Task Handler_GetLatestRates_ShouldReturnRateFromExchangeRateService()
    {
        // Arrange
        var rate = new CurrencyRate("GBP", 0.79m, DateTimeOffset.UtcNow);
        exchangeRate.Setup(e => e.GetLatestRatesAsync("USD", "GBP")).ReturnsAsync(rate);
        var handler = new GetExchangeRatesQueryHandler(exchangeRate.Object);

        // Act
        var result = await handler.Handle(new GetExchangeRatesQuery("GBP"), default);

        // Assert
        result.ShouldBe(rate);
    }

    [Fact]
    public async Task Handler_GetLatestRatesWhenExchangeRateServiceFails_ShouldPropagateException()
    {
        // Arrange
        exchangeRate.Setup(e => e.GetLatestRatesAsync(It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("Frankfurter returned no exchange rates."));
        var handler = new GetExchangeRatesQueryHandler(exchangeRate.Object);

        // Act
        async Task Action()
        {
            await handler.Handle(new GetExchangeRatesQuery("ZZZ"), default);
        }

        // Assert
        await Should.ThrowAsync<InvalidOperationException>((Func<Task>)Action);
    }
}
