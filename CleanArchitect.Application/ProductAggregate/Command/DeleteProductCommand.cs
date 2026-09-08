using MediatR;

namespace CleanArchitect.Application.ProductAggregate.Command;

public sealed record DeleteProductCommand(Guid Id) : IRequest;
