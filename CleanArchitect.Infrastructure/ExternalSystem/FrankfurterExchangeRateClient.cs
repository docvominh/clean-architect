using System.Collections.Concurrent;
using System.Net.Http.Json;

using CleanArchitect.Application.ExternalSystem;

namespace CleanArchitect.Infrastructure.ExternalSystem;

public sealed class FrankfurterExchangeRateClient(IHttpClientFactory httpClientFactory) : IFrankfurterExchangeRateClient
{
    public const string HttpClientName = "Frankfurter";

    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    private readonly ConcurrentDictionary<string, CurrencyRate> exchangeRates = [];

    public async Task<CurrencyRate> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
    {
        var cacheKey = $"{baseCurrency}:{targetCurrency}";

        if (exchangeRates.TryGetValue(cacheKey, out var cachedRate) && DateTimeOffset.Now - cachedRate.Time < CacheDuration)
        {
            return cachedRate;
        }

        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        var response = await httpClient.GetFromJsonAsync<FrankfurterRatesResponse>($"rate/{baseCurrency}/{targetCurrency}");

        if (response == null)
        {
            throw new InvalidOperationException("Frankfurter returned no exchange rates.");
        }

        var responseRate = new CurrencyRate(response.Quote, response.Rate, DateTimeOffset.Now);

        exchangeRates[cacheKey] = responseRate;

        return responseRate;
    }
}
