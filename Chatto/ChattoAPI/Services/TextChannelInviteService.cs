using System.Security.Claims;
using Chatto.Infrastructure;
using Microsoft.AspNetCore.Authorization;

namespace Chatto.Services;

public interface ITextChannelInviteService
{
    Task CreateInviteAsync(User author, User target, TextChannel textChannel);
}

public class TextChannelInviteService : ITextChannelInviteService
{
    private readonly DatabaseContext _databaseContext;

    public TextChannelInviteService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task CreateInviteAsync(User author, User target, TextChannel textChannel)
    {   
        var invite = new TextChannelInvite()
        {
            Author = author,
            Target = target,
            TextChannel = textChannel
        };

        _databaseContext.TextChannelInvites.Add(invite);
        await _databaseContext.SaveChangesAsync();
    }
}