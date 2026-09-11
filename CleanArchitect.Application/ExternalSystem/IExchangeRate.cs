namespace CleanArchitect.Application.ExternalSystem;

public interface IExchangeRate
{
    Task<IReadOnlyDictionary<string, decimal>> GetLatestRatesAsync(
        string baseCurrency,
        IReadOnlyCollection<string> targetCurrencies,
        CancellationToken cancellationToken);
}
