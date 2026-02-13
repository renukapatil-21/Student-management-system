using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StudentManagementSystem.Application.Features.Users.Commands.CreateUser;
using StudentManagementSystem.Application.Features.Users.Commands.LoginUser;
using StudentManagementSystem.Application.DTOs.Users;

namespace StudentManagementSystem.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : BaseController
{
    public AuthController(IMediator mediator) : base(mediator) { }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginDto loginDto)
    {
        var command = new LoginUserCommand { LoginData = loginDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpPost("register")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Register([FromBody] CreateUserDto userDto)
    {
        var command = new CreateUserCommand { User = userDto };
        var result = await _mediator.Send(command);
        return HandleResult(result);
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst("sub")?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized();
        }

        // Implementation would get user details by ID
        return Ok(new { Message = "User authenticated", UserId = userId });
    }
}