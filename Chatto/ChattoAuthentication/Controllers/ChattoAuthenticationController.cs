using ChattoAuth.Models;
using ChattoAuth.Services;
using Microsoft.AspNetCore.Mvc;

namespace ChattoAuth.Controllers;

[ApiController]
[Route("api/Account")]
public class ChattoAuthenticationController : Controller
{
    private readonly IChattoAuthenticationService _authenticationService;
    private readonly IAccountService _accountService;
    
    public ChattoAuthenticationController(
        IChattoAuthenticationService authenticationService, 
        IAccountService accountService)
    {
        _authenticationService = authenticationService;
        _accountService = accountService;
    }

    [HttpPost]
    [Route("RegisterChatto")]
    public async Task<IActionResult> RegisterChatto([FromBody] ChattoAccountAuthenticationDataModel dataModel)
    {
        var callbackData = await _authenticationService.RegisterAsync(dataModel);

        var account = await _authenticationService.AuthenticateAsync(callbackData.AccountId, callbackData.Password);

        var jwt = await _accountService.GetAccountJwtAsync(account);
        
        return Ok(jwt);
    }

    [HttpPost]
    [Route("LoginChatto")]
    public async Task<IActionResult> LoginChatto()
    {
        var account = await _authenticationService.AuthenticateAsync(Request);
        
        var jwt = await _accountService.GetAccountJwtAsync(account);
        
        return Ok(jwt);
    }
}