using ChattoAuth.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChattoAuth.Controllers;

[ApiController]
[Route("api/Account")]
public class GoogleAuthenticationController : Controller
{
    private readonly IGoogleAuthenticationService _googleAuthenticationService;
    private readonly IAccountService _accountService;

    // GET
    public GoogleAuthenticationController(
        IGoogleAuthenticationService googleAuthenticationService, 
        IAccountService accountService)
    {
        _googleAuthenticationService = googleAuthenticationService;
        _accountService = accountService;
    }

    [HttpPost]
    [Route("RegisterGoogle")]
    public async Task<IActionResult> RegisterGoogle()
    {
        Request.Headers.TryGetValue("Authorization", out var jwt);
        await _accountService.RegisterGoogleAccount(jwt);
        return Ok();
    }

    [HttpGet]
    [Route("LoginGoogle")]
    public async Task<IActionResult> LoginGoogle()
    {
        Request.Headers.TryGetValue("Authorization", out var googleJwt);
        
        var account = await _googleAuthenticationService.Authenticate(googleJwt);
        var jwt = await _accountService.GetJwT(account);
        
        return Ok(new
        {
            jwt = jwt
        });
    }
}