using CleanArchitect.Application.ExternalSystem;
using CleanArchitect.Application.ExternalSystem.Query;

using Moq;

using Shouldly;

using Xunit;

namespace CleanArchitect.Application.UnitTests.ExternalSystem;

public class GetExchangeRatesQueryHandlerTests
{
    private readonly Mock<IExchangeRate> exchangeRate = new();

    [Fact]
    public async Task Handler_GetLatestRates_ShouldReturnRatesFromExchangeRateService()
    {
        // Arrange
        var rates = new Dictionary<string, decimal> { ["GBP"] = 0.79m, ["EUR"] = 0.92m };
        exchangeRate.Setup(e => e.GetLatestRatesAsync("USD", It.Is<IReadOnlyCollection<string>>(c => c.SequenceEqual(new[] { "GBP", "EUR" })), It.IsAny<CancellationToken>()))
            .ReturnsAsync(rates);
        var handler = new GetExchangeRatesQueryHandler(exchangeRate.Object);

        // Act
        var result = await handler.Handle(new GetExchangeRatesQuery(), default);

        // Assert
        result.Rates.ShouldBe(rates);
    }

    [Fact]
    public async Task Handler_GetLatestRatesWhenExchangeRateServiceFails_ShouldPropagateException()
    {
        // Arrange
        exchangeRate.Setup(e => e.GetLatestRatesAsync(It.IsAny<string>(), It.IsAny<IReadOnlyCollection<string>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Frankfurter returned no exchange rates."));
        var handler = new GetExchangeRatesQueryHandler(exchangeRate.Object);

        // Act
        async Task Action()
        {
            await handler.Handle(new GetExchangeRatesQuery(), default);
        }

        // Assert
        await Should.ThrowAsync<InvalidOperationException>((Func<Task>)Action);
    }
}
