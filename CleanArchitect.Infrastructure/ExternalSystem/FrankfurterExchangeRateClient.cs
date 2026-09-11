using System.Net.Http.Json;

using CleanArchitect.Application.ExternalSystem;

namespace CleanArchitect.Infrastructure.ExternalSystem;

public sealed class FrankfurterExchangeRateClient(HttpClient httpClient) : IExchangeRate
{
    public async Task<IReadOnlyDictionary<string, decimal>> GetLatestRatesAsync(
        string baseCurrency,
        IReadOnlyCollection<string> targetCurrencies,
        CancellationToken cancellationToken)
    {
        var symbols = string.Join(',', targetCurrencies);
        var response = await httpClient.GetFromJsonAsync<FrankfurterRatesResponse>($"latest?base={baseCurrency}&symbols={symbols}", cancellationToken);

        return response?.Rates ?? throw new InvalidOperationException("Frankfurter returned no exchange rates.");
    }

    private sealed record FrankfurterRatesResponse
    {
        public string? Base { get; init; }
        public required IReadOnlyDictionary<string, decimal> Rates { get; init; }
    }
}
