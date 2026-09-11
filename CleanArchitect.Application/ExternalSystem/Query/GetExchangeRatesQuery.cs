using MediatR;

namespace CleanArchitect.Application.ExternalSystem.Query;

public sealed record GetExchangeRatesQuery : IRequest<ExchangeRatesDto>;
