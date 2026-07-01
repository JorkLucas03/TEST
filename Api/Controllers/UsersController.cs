using Api.Features.Users.CreateUser;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class UsersController : BaseApiController
{
    [HttpPost]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await Mediator.Send(command);
        return CreatedAtAction(nameof(CreateUser), new { id = userId }, userId);
    }
}
