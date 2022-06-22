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
    
    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] TextChannelModel textChannelModel)
    {
        var presentUser = await _userService.GetUserAsync(User);
        
        await _textChannelService.CreatNewTextChannel(textChannelModel, presentUser.Guid);
        
        return Ok();
    }

    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    [HttpPost("{textChannelGuid}")]
    public async Task<IActionResult> SendMessage(
        [FromRoute] Guid textChannelGuid, 
        [FromBody] SendMessageModel sendMessageModel)
    {
        var presentUser = await _userService.GetUserAsync(User);
        var textChannel = await _textChannelService.GetUsers(textChannelGuid);

        // TODO: this shit is not doing anything wtf?
        await _authorizationService.AuthorizeAsync(User, textChannel, Operations.SendMessage);
        
        var message =
            await _textChannelService.SendMessageToChannel(textChannelGuid, sendMessageModel, presentUser.Guid);
        
        return Ok(message);
    }

    [Authorize(Policy = AuthorizationPolicies.Authenticated)]
    [HttpGet("{textChannelGuid:guid}")]
    public async Task<IActionResult> GetMessages([FromRoute] Guid textChannelGuid)
    {
        var textChannel = await _textChannelService.GetUsers(textChannelGuid);

        var authResult = await _authorizationService.AuthorizeAsync(User, textChannel, Operations.View);
        if (!authResult.Succeeded)
            return Forbid();

        var messages = await _textChannelService.GetMessages(textChannelGuid);

        return Ok(messages);
    }
}