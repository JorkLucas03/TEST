using Api.Features.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[Route("api/users")]
[Route("api/usuarios")]
public class UsersController : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<UserResponse>>> GetUsers(
        [FromQuery] GetUsersQuery query
    )
    {
        var users = await Mediator.Send(query ?? new GetUsersQuery());
        return Ok(users);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await Mediator.Send(command);
        return CreatedAtAction(nameof(CreateUser), new { id = userId }, userId);
    }
}
