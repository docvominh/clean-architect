using System.Net.Http.Json;

using CleanArchitect.Application.ExternalSystem;

namespace CleanArchitect.Infrastructure.ExternalSystem;

public sealed class FrankfurterExchangeRateClient(IHttpClientFactory httpClientFactory) : IFrankfurterExchangeRateClient
{
    public const string HttpClientName = "Frankfurter";

    public async Task<CurrencyRate> GetExchangeRateAsync(string baseCurrency, string targetCurrency)
    {
        var httpClient = httpClientFactory.CreateClient(HttpClientName);
        var response = await httpClient.GetFromJsonAsync<FrankfurterRatesResponse>($"rate/{baseCurrency}/{targetCurrency}");

        if (response == null)
        {
            throw new InvalidOperationException("Frankfurter returned no exchange rates.");
        }

        return new CurrencyRate(response.Quote, response.Rate, DateTimeOffset.Now);
    }
}
