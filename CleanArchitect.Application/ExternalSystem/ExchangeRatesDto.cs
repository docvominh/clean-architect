namespace CleanArchitect.Application.ExternalSystem;

public sealed record ExchangeRatesDto(IReadOnlyDictionary<string, decimal> Rates);
