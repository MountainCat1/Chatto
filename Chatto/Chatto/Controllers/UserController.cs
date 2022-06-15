using Chatto.Models;
using Chatto.Services;
using Microsoft.AspNetCore.Mvc;

namespace Chatto.Controllers;

[ApiController]
[Route("api/User")]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    [Route("LoginGoogle")]
    public async Task<IActionResult> LoginGoogle()
    {
        var token = await _userService.GetUser(Request);
        return Ok(token);
    }
}