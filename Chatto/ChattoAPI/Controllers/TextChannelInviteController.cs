using Chatto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Chatto.Controllers;

[ApiController]
[Route("api/Invite")]
[Authorize(Policy = AuthorizationPolicies.Authenticated)]
public class TextChannelInviteController : Controller
{
    private readonly ITextChannelInviteService _textChannelInviteService;
    private readonly IUserService _userService;
    private readonly ITextChannelService _textChannelService;
    private readonly IAuthorizationService _authorizationService;

    public TextChannelInviteController(
        ITextChannelInviteService textChannelInviteService,
        IUserService userService, 
        ITextChannelService textChannelService, 
        IAuthorizationService authorizationService)
    {
        _textChannelInviteService = textChannelInviteService;
        _userService = userService;
        _textChannelService = textChannelService;
        _authorizationService = authorizationService;
    }
    
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    [HttpPost("/Create/{channelGuid}/{targetGuid}")]
    public async Task<IActionResult> Create(
        [FromRoute] Guid channelGuid, 
        [FromRoute] Guid targetGuid)
    {
        var authorUser = await _userService.GetUserAsync(User);
        
        var authResult = await _authorizationService.AuthorizeAsync(User, Operations.InviteNewMembers);
        if (!authResult.Succeeded)
            return Forbid();
        
        await _textChannelInviteService.CreateInviteAsync(authorUser.Guid, targetGuid, channelGuid);
        
        return Ok();
    }

    public async Task<IActionResult> Accept()
    {
        return Ok();
    }

    public async Task<IActionResult> Decline()
    {
        return Ok();
    }

    public async Task<IActionResult> List()
    {
        return Ok();
    }
}