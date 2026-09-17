using MediatR;

namespace CleanArchitect.Application.ExternalSystem.Query;

public sealed record GetExchangeRatesQuery(string TargetCurrency) : IRequest<CurrencyRate>;
