using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Command;

public sealed record UpdateProductCommand(Guid Id, ProductRequest Request, Guid UpdatedBy) : IRequest<ProductResponse>;
