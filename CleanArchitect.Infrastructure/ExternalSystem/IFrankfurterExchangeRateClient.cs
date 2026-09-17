using CleanArchitect.Application.ExternalSystem;

namespace CleanArchitect.Infrastructure.ExternalSystem;

public interface IFrankfurterExchangeRateClient
{
    public Task<CurrencyRate> GetExchangeRateAsync(string baseCurrency, string targetCurrency);
}
