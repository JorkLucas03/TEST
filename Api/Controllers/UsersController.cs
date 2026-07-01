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

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<UserResponse>> GetUserById(Guid id)
    {
        var user = await Mediator.Send(new GetUserByIdQuery(id));
        return Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> CreateUser([FromBody] CreateUserCommand command)
    {
        var userId = await Mediator.Send(command);
        return CreatedAtAction(nameof(CreateUser), new { id = userId }, userId);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserCommand command)
    {
        if (id != command.Id)
        {
            return BadRequest("El identificador de la URL no coincide con el del cuerpo.");
        }

        await Mediator.Send(command);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteUser(Guid id)
    {
        await Mediator.Send(new DeleteUserCommand(id));
        return NoContent();
    }
}
