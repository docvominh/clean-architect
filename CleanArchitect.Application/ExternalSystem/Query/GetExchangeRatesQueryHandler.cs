using MediatR;

namespace CleanArchitect.Application.ExternalSystem.Query;

public sealed class GetExchangeRatesQueryHandler(IExchangeRateService exchangeRateService) : IRequestHandler<GetExchangeRatesQuery, CurrencyRate>
{
    private const string BaseCurrency = "USD";

    public async Task<CurrencyRate> Handle(GetExchangeRatesQuery request, CancellationToken cancellationToken)
    {
        return await exchangeRateService.GetLatestRatesAsync(BaseCurrency, request.TargetCurrency);
    }
}
