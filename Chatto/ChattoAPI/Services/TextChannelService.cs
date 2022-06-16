using System.Net.Mime;
using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface ITextChannelService
{
    Task CreatNewTextChannel(TextChannel textChannel);
    Task AddMessage(Guid textChannelGuid, Guid messageGuid);
    Task<ICollection<Message>> GetMessages(Guid textChannelGuid);
}

public class TextChannelService : ITextChannelService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<ITextChannelService> _logger;
    private readonly IGuidClient _guidClient;

    public TextChannelService(
        DatabaseContext databaseContext, 
        ILogger<ITextChannelService> logger, 
        IGuidClient guidClient)
    {
        _databaseContext = databaseContext;
        _logger = logger;
        _guidClient = guidClient;
    }

    public async Task CreatNewTextChannel(TextChannel textChannel)
    {
        textChannel.Guid = await _guidClient.GetGuidAsync();
        
        await _databaseContext.TextChannels.AddAsync(textChannel);
        
        _logger.LogInformation($"Creating text channel ({textChannel.Guid})");
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
        return (await _databaseContext.TextChannels.FindAsync(textChannelGuid)).Messages;
    }
}