using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Chatto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Chatto.Controllers;

[ApiController]
[Route("api/Authentication")]
public class UserController : Controller
{
    private readonly IAuthenticationService _authenticationService;
    private readonly IUserService _userService;
    
    public UserController(IAuthenticationService authenticationService, IUserService userService)
    {
        _authenticationService = authenticationService;
        _userService = userService;
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("LoginGoogle")]
    public async Task<IActionResult> LoginGoogle()
    {
        var tokenString = await _authenticationService.LoginUserGoogle(Request);
        return Ok(tokenString);
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("RegisterGoogle")]
    public async Task<IActionResult> RegisterGoogle([FromBody] GoogleRegisterModel registerModel)
    {
        var tokenString = await _authenticationService.RegisterUserGoogle(registerModel);
        
        return Ok(tokenString);
    }
    
    [Authorize]
    [HttpGet]
    [Route("WhoAmI")]
    public async Task<IActionResult> WhoAmI()
    {
        var claim = User.FindFirst(u => u.Type == ClaimTypes.NameIdentifier);
        var userEntity = await _userService.GetUserAsync(int.Parse(claim.Value));
        
        return Ok($"{userEntity.Username} {userEntity.Guid}");
    }
    
    [AllowAnonymous]
    [HttpGet]
    [Route("IsApiAlive")]
    public async Task<IActionResult> AuthorizationCheck()
    {
        return Ok("YES!");
    }
}