using System.Net.Mime;
using AutoMapper;
using Chatto.Infrastructure;
using Chatto.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface ITextChannelService
{
    Task<Guid> CreatNewTextChannelAsync(TextChannelModel model, Guid memberGuid);
    Task AddUserAsync(Guid textChannelGuid, Guid userGuid);
    Task AddMessageAsync(Guid textChannelGuid, Guid messageGuid);
    Task<ICollection<MessageModel>> GetMessagesAsync(Guid textChannelGuid);
    Task<TextChannel> GetTextChannelAsync(Guid textChannelGuid);
    Task<TextChannel> GetUsersAsync(Guid textChannelGuid);
    Task<MessageModel> SendMessageToChannelAsync(Guid textChannelGuid, SendMessageModel sendMessageModel, Guid authorUserGuid);
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

    public async Task<Guid> CreatNewTextChannelAsync(TextChannelModel model, Guid memberGuid)
    {
        var textChannelEntity = _mapper.Map<TextChannel>(model);
        var userMember = await _userService.GetUserAsync(memberGuid);
        var guid = await _guidClient.GetGuidAsync();
        
        textChannelEntity.Users = new List<User>{userMember};
        textChannelEntity.Guid = guid;
        
        await _databaseContext.TextChannels.AddAsync(textChannelEntity);
        
        _logger.LogInformation(
            $"Creating text channel ({textChannelEntity.ChannelType.ToString()},{textChannelEntity.Guid})");
        await _databaseContext.SaveChangesAsync();

        return guid;
    }

    public async Task AddUserAsync(Guid textChannelGuid, Guid userGuid)
    {
        var textChannel = await GetUsersAsync(textChannelGuid);
        var user = await _userService.GetUserAsync(userGuid);
        
        textChannel.Users.Add(user);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task AddMessageAsync(Guid textChannelGuid, Guid messageGuid)
    {
        var textChannel = await _databaseContext.TextChannels
            .Include(x => x.Messages)
            .FirstAsync(x => x.Guid == textChannelGuid);
        var message = await _databaseContext.Messages.FindAsync(messageGuid);

        textChannel.Messages.Add(message);
        
        _logger.LogInformation($"Adding message ({message.Guid}) to text channel ({textChannel.Guid})");
        await _databaseContext.SaveChangesAsync();
    }

    public async Task<ICollection<MessageModel>> GetMessagesAsync(Guid textChannelGuid)
    {
        var textChannel = await _databaseContext.TextChannels
            .Include(channel => channel.Messages)
            .AsNoTracking()
            .FirstAsync(channel => channel.Guid == textChannelGuid);

        var models = _mapper.Map<List<MessageModel>>(textChannel.Messages);
        
        return models;
    }

    public async Task<TextChannel> GetTextChannelAsync(Guid textChannelGuid)
    {
        return await _databaseContext.TextChannels
            .Include(channel => channel.Messages)
            .FirstAsync(channel => channel.Guid == textChannelGuid);
    }

    public async Task<TextChannel> GetUsersAsync(Guid textChannelGuid)
    {
        return _databaseContext.TextChannels
            .Include(channel => channel.Users)
            .First(channel => channel.Guid == textChannelGuid);
    }

    public async Task<MessageModel> SendMessageToChannelAsync(Guid textChannelGuid, SendMessageModel sendMessageModel, Guid authorUserGuid)
    {
        var textChannel = await GetTextChannelAsync(textChannelGuid);
        var messageEntity = _mapper.Map<Message>(sendMessageModel);
        var authorUser = await _userService.GetUserAsync(authorUserGuid);
        var messageGuid = await _guidClient.GetGuidAsync();
        
        messageEntity.Author = authorUser;
        messageEntity.Guid = messageGuid;
        messageEntity.Time = DateTime.Now;

        await _databaseContext.Messages.AddAsync(messageEntity);
        textChannel.Messages.Add(messageEntity);
        await _databaseContext.SaveChangesAsync();

        return _mapper.Map<MessageModel>(messageEntity);
    }
}