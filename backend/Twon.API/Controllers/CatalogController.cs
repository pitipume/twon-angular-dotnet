using MediatR;
using Microsoft.AspNetCore.Mvc;
using Twon.Application.Catalog.Queries.GetProducts;
using Twon.Application.Catalog.Queries.GetProductById;

namespace Twon.API.Controllers;

[ApiController]
[Route("api/catalog")]
public class CatalogController(IMediator mediator) : ControllerBase
{
    // GET /api/catalog/products?type=ebook
    [HttpGet("products")]
    public async Task<IActionResult> GetProducts([FromQuery] string? type)
    {
        var result = await mediator.Send(new GetProductsQuery(type));
        return Ok(result);
    }

    // GET /api/catalog/products/:id
    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetProduct(string id)
    {
        var result = await mediator.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }
}
