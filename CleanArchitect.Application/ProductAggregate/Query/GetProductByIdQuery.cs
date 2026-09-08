using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Query;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductResponse>;
