using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Query;

public sealed record GetProductsQuery : IRequest<List<ProductResponse>>;
