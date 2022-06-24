using System.Security.Claims;
using Chatto.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface ITextChannelInviteService
{
    Task CreateInviteAsync(Guid authorGuid, Guid targetGuid, Guid textChannelGuid)
    Task AcceptInviteAsync(Guid inviteGuid);
    Task DeclineInviteAsync(Guid inviteGuid);
    Task<TextChannelInvite> GetInviteAsync(Guid inviteGuid);
    Task<List<TextChannelInvite>> GetPendingInvitesAsync(Guid userGuid);
    Task<List<TextChannelInvite>> GetSentInvitesAsync(Guid userGuid);
}

public class TextChannelInviteService : ITextChannelInviteService
{
    private readonly DatabaseContext _databaseContext;
    private readonly ITextChannelService _textChannelService;
    private readonly IUserService _userService;

    public TextChannelInviteService(
        DatabaseContext databaseContext, 
        ITextChannelService textChannelService, 
        IUserService userService)
    {
        _databaseContext = databaseContext;
        _textChannelService = textChannelService;
        _userService = userService;
    }

    public async Task CreateInviteAsync(Guid authorGuid, Guid targetGuid, Guid textChannelGuid)
    {   
        var author = await _userService.GetUserAsync(authorGuid);
        var target = await _userService.GetUserAsync(targetGuid);
        var textChannel = await _textChannelService.GetTextChannelAsync(textChannelGuid);
        
        var invite = new TextChannelInvite()
        {
            Author = author,
            Target = target,
            TextChannel = textChannel
        };

        _databaseContext.TextChannelInvites.Add(invite);
        await _databaseContext.SaveChangesAsync();
    }

    public async Task AcceptInviteAsync(Guid inviteGuid)
    {
        var invite = await GetInviteAsync(inviteGuid);
        await _textChannelService.AddUserAsync(invite.TargetGuid, invite.TextChannelGuid);
    }

    public async Task DeclineInviteAsync(Guid inviteGuid)
    {
        throw new NotImplementedException();
    }

    public async Task<TextChannelInvite> GetInviteAsync(Guid inviteGuid)
    {
        return await _databaseContext.TextChannelInvites
            .FirstAsync(invite => invite.Guid == inviteGuid);
    }

    public async Task<List<TextChannelInvite>> GetPendingInvitesAsync(Guid userGuid)
    {
        return await _databaseContext.TextChannelInvites
            .Where(invite => invite.TargetGuid == userGuid)
            .ToListAsync();
    }

    public async Task<List<TextChannelInvite>> GetSentInvitesAsync(Guid userGuid)
    {
        return await _databaseContext.TextChannelInvites
            .Where(invite => invite.AuthorGuid == userGuid)
            .ToListAsync();
    }
}