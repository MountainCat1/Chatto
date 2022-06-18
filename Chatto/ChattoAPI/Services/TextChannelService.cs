using System.Net.Mime;
using AutoMapper;
using Chatto.Infrastructure;
using Chatto.Models;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Chatto.Services;

public interface ITextChannelService
{
    Task CreatNewTextChannel(TextChannelModel model, Guid memberGuid);
    Task AddMessage(Guid textChannelGuid, Guid messageGuid);
    Task<ICollection<Message>> GetMessages(Guid textChannelGuid);
    Task<TextChannel> GetTextChannel(Guid textChannelGuid);
}

public class TextChannelService : ITextChannelService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<ITextChannelService> _logger;
    private readonly IGuidClient _guidClient;
    private readonly IUserService _userService;

    private readonly IMapper _mapper;

    public TextChannelService(
        DatabaseContext databaseContext, 
        ILogger<ITextChannelService> logger, 
        IGuidClient guidClient, IMapper mapper, IUserService userService)
    {
        _databaseContext = databaseContext;
        _logger = logger;
        _guidClient = guidClient;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task CreatNewTextChannel(TextChannelModel model, Guid memberGuid)
    {
        var textChannelEntity = _mapper.Map<TextChannel>(model);
        var userMember = await _userService.GetUserAsync(memberGuid);

        textChannelEntity.Users = new List<User>{userMember};
        textChannelEntity.Guid = await _guidClient.GetGuidAsync();
        
        await _databaseContext.TextChannels.AddAsync(textChannelEntity);
        
        _logger.LogInformation(
            $"Creating text channel ({textChannelEntity.ChannelType.ToString()},{textChannelEntity.Guid})");
        await _databaseContext.SaveChangesAsync();
    }
    
    public async Task AddMessage(Guid textChannelGuid, Guid messageGuid)
    {
        var textChannel = await _databaseContext.TextChannels
            .Include(x => x.Messages)
            .FirstAsync(x => x.Guid == textChannelGuid);
        var message = await _databaseContext.Messages.FindAsync(messageGuid);

        textChannel.Messages.Add(message);
        
        _logger.LogInformation($"Adding message ({message.Guid}) to text channel ({textChannel.Guid})");
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<ICollection<Message>> GetMessages(Guid textChannelGuid)
    {
        var textChannel = await _databaseContext.TextChannels.FindAsync(textChannelGuid);
        return textChannel.Messages;
    }

    public async Task<TextChannel> GetTextChannel(Guid textChannelGuid)
    {
        return await _databaseContext.TextChannels.FindAsync(textChannelGuid);
    }
}