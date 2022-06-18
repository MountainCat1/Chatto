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

    private readonly IMapper _mapper;

    public TextChannelController(ITextChannelService textChannelService, IUserService userService, IMapper mapper)
    {
        _textChannelService = textChannelService;
        _userService = userService;
        _mapper = mapper;
    }

    [Authorize(Policy = "Authenticated")]
    [HttpGet("List")]
    public async Task<IActionResult> List()
    {
        var presentUser = await _userService.GetUserAsync(User);
        var textChannels = await _userService.GetUserTextChannelsAsync(presentUser.Guid); 
        return Ok(textChannels);
    }
    
    [Authorize(Policy = "Authenticated")]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] TextChannelModel textChannelModel)
    {
        var presentUser = await _userService.GetUserAsync(User);
        
        await _textChannelService.CreatNewTextChannel(textChannelModel, presentUser.Guid);
        
        return Ok();
    }
}