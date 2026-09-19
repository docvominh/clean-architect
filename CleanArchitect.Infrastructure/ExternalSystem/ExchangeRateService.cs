using CleanArchitect.Application.ExternalSystem;

using Microsoft.Extensions.Caching.Memory;

namespace CleanArchitect.Infrastructure.ExternalSystem;

public sealed class ExchangeRateService(
    IFrankfurterExchangeRateClient frankfurterExchangeRateClient,
    IMemoryCache memoryCache) : IExchangeRateService
{
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(30);

    public async Task<CurrencyRate> GetLatestRatesAsync(string baseCurrency, string targetCurrency)
    {
        var cacheKey = $"{nameof(ExchangeRateService)}:{baseCurrency}:{targetCurrency}";

        var cachedRate = await memoryCache.GetOrCreateAsync(
            cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = CacheDuration;

                return await frankfurterExchangeRateClient.GetExchangeRateAsync(baseCurrency, targetCurrency);
            });

        return cachedRate!;
    }
}
