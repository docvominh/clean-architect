namespace CleanArchitect.Application.ExternalSystem;

public record CurrencyRate(string Symbol, decimal Rate, DateTimeOffset Time);
