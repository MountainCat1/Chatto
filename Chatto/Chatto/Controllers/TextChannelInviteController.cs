using Chatto.Models;
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
    

    [HttpPost("Create")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> Create([FromBody] TextChannelInviteModel inviteModel)
    {
        var authorUser = await _userService.GetUserAsync(User);
        var channel = await _textChannelService.GetUsersAsync(inviteModel.TextChannelGuid);
        
        var authResult = await _authorizationService.AuthorizeAsync(User, channel, Operations.InviteNewMembers);
        if (!authResult.Succeeded)
            return Forbid();
        
        await _textChannelInviteService.CreateInviteAsync(
            authorUser.Guid, 
            inviteModel.TargetUserGuid, 
            inviteModel.TextChannelGuid);
        
        return Ok();
    }
    
    [HttpGet("List")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> List()
    {
        var user = await _userService.GetUserAsync(User);
        
        var invites = await _textChannelInviteService.GetPendingInvitesAsync(user.Guid);
        
        return Ok(invites);
    }
    
    [HttpPost("Accept/{inviteGuid}")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> Accept([FromRoute] Guid inviteGuid)
    {

        var invite = await _textChannelInviteService.GetInviteAsync(inviteGuid);
        
        // Authorize
        var authResult = await _authorizationService.AuthorizeAsync(User, invite, Operations.AcceptInvite);
        if (!authResult.Succeeded)
            return Forbid();
        
        await _textChannelInviteService.AcceptInviteAsync(inviteGuid);
        
        return Ok();
    }

    [HttpPost("Decline/{channelGuid}")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> Decline()
    {
        return Ok();
    }
    
    
}