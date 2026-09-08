using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Command;

public sealed record CreateProductCommand(ProductRequest Request, Guid CreatedBy) : IRequest<ProductResponse>;
