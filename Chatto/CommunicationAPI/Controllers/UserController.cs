using CommunicationAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace CommunicationAPI.Controllers;

[ApiController]
[Route("api/User")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserModel createModel)
    {
        await _userService.CreateUserAsync(createModel);

        return Ok();
    }
}