namespace CleanArchitect.Application.ExternalSystem;

public interface IExchangeRateService
{
    Task<CurrencyRate> GetLatestRatesAsync(string baseCurrency, string targetCurrency);
}
