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
        Request.Headers.TryGetValue("Authorization", out var googleJwt);
        await _googleAuthenticationService.RegisterAsync(new GoogleAuthenticationData()
        {
            Jwt = googleJwt
        });

        return await LoginGoogle();
    }

    [HttpGet]
    [Route("LoginGoogle")]
    public async Task<IActionResult> LoginGoogle()
    {
        var account = await _googleAuthenticationService.AuthenticateAsync(Request);
        
        var jwt = await _accountService.GetAccountJwtAsync(account);
        
        return Ok(jwt);
    }
}