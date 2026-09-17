using CleanArchitect.Application.ExternalSystem;

namespace CleanArchitect.Infrastructure.ExternalSystem;

public sealed class ExchangeRateService(IFrankfurterExchangeRateClient frankfurterExchangeRateClient) : IExchangeRateService
{
    public async Task<CurrencyRate> GetLatestRatesAsync(string baseCurrency, string targetCurrency)
    {
        return await frankfurterExchangeRateClient.GetExchangeRateAsync(baseCurrency, targetCurrency);
    }
}
