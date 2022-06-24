using AutoMapper;
using Chatto.Infrastructure;
using Chatto.Models;
using Chatto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Models;

namespace Chatto.Controllers;

[ApiController]
[Route("api/TextChannel")]
public class TextChannelController : Controller
{
    private readonly ITextChannelService _textChannelService;
    private readonly IUserService _userService;
    private readonly IAuthorizationService _authorizationService;

    private readonly IMapper _mapper;

    public TextChannelController(
        ITextChannelService textChannelService, 
        IUserService userService, 
        IMapper mapper, 
        IAuthorizationService authorizationService)
    {
        _textChannelService = textChannelService;
        _userService = userService;
        _mapper = mapper;
        _authorizationService = authorizationService;
    }

    [HttpGet("List")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> List()
    {
        var presentUser = await _userService.GetUserAsync(User);
        var textChannels = await _userService.GetUserTextChannelsAsync(presentUser.Guid); 
        return Ok(textChannels);
    }
    

    [HttpPost("Create")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> Create([FromBody] TextChannelModel textChannelModel)
    {
        var presentUser = await _userService.GetUserAsync(User);
        
        await _textChannelService.CreatNewTextChannelAsync(textChannelModel, presentUser.Guid);
        
        return Ok();
    }

    [HttpPost("{textChannelGuid}")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> SendMessage(
        [FromRoute] Guid textChannelGuid, 
        [FromBody] SendMessageModel sendMessageModel)
    {
        var presentUser = await _userService.GetUserAsync(User);
        var textChannel = await _textChannelService.GetUsersAsync(textChannelGuid);
     
        // Authorize
        var authResult = await _authorizationService.AuthorizeAsync(User, textChannel, Operations.SendMessage);
        if (!authResult.Succeeded)
            return Forbid();
        
        var message =
            await _textChannelService.SendMessageToChannelAsync(textChannelGuid, sendMessageModel, presentUser.Guid);
        
        return Ok(message);
    }
    
    [HttpGet("{textChannelGuid:guid}")]
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    public async Task<IActionResult> GetMessages([FromRoute] Guid textChannelGuid)
    {
        var textChannel = await _textChannelService.GetUsersAsync(textChannelGuid);
        
        // Authorize
        var authResult = await _authorizationService.AuthorizeAsync(User, textChannel, Operations.View);
        if (!authResult.Succeeded)
            return Forbid();

        var messages = await _textChannelService.GetMessagesAsync(textChannelGuid);

        return Ok(messages);
    }
}