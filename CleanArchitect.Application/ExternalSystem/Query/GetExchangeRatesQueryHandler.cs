using MediatR;

namespace CleanArchitect.Application.ExternalSystem.Query;

public sealed class GetExchangeRatesQueryHandler(IExchangeRate exchangeRate) : IRequestHandler<GetExchangeRatesQuery, ExchangeRatesDto>
{
    private const string BaseCurrency = "USD";
    private static readonly string[] TargetCurrencies = ["GBP", "EUR"];

    public async Task<ExchangeRatesDto> Handle(GetExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        var rates = await exchangeRate.GetLatestRatesAsync(BaseCurrency, TargetCurrencies, cancellationToken);

        return new ExchangeRatesDto(rates);
    }
}
