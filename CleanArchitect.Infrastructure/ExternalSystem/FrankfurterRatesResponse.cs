namespace CleanArchitect.Infrastructure.ExternalSystem;

public sealed class FrankfurterRatesResponse
{
    public required string Base { get; init; }
    public required string Quote { get; init; }
    public required decimal Rate { get; init; }
}
