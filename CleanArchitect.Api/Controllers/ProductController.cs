using System.Security.Claims;

using CleanArchitect.Application.ProductAggregate;
using CleanArchitect.Application.ProductAggregate.Command;
using CleanArchitect.Application.ProductAggregate.Query;

using MediatR;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CleanArchitect.Api.Controllers;

[ApiController]
[Route("api/products")]
[Produces("application/json")]
public class ProductController(ISender sender) : ControllerBase
{
    [HttpGet]
    public Task<ProductsDto> GetAll(CancellationToken cancellationToken)
    {
        return sender.Send(new GetProductsQuery(), cancellationToken);
    }

    [HttpGet("{id:guid}")]
    public Task<ProductDto> GetById(Guid id, CancellationToken cancellationToken)
    {
        return sender.Send(new GetProductByIdQuery(id), cancellationToken);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public Task<ProductDto> Create(ProductRequest request, CancellationToken cancellationToken)
    {
        var createdBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return sender.Send(new CreateProductCommand(request, createdBy), cancellationToken);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public Task<ProductDto> Update(Guid id, ProductRequest request, CancellationToken cancellationToken)
    {
        var updatedBy = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        return sender.Send(new UpdateProductCommand(id, request, updatedBy), cancellationToken);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await sender.Send(new DeleteProductCommand(id), cancellationToken);

        return NoContent();
    }
}
