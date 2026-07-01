using Api.Features.Products.CreateProduct;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class ProductsController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductCommand command)
    {
        var productId = await Mediator.Send(command);
        return CreatedAtAction(nameof(CreateProduct), new { id = productId }, productId);
    }
}
