using Api.Features.Products;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/products")]
[Route("api/productos")]
public class ProductsController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<ProductResponse>>> GetProducts()
    {
        var products = await Mediator.Send(new GetProductsQuery());
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await Mediator.Send(command);
        return CreatedAtAction(nameof(CreateProduct), new { id = productId }, productId);
    }
}
