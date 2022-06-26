using System.Security.Claims;
using Chatto.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface ITextChannelInviteService
{
    Task CreateInviteAsync(Guid authorGuid, Guid targetGuid, Guid textChannelGuid);
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
    private readonly ILogger<ITextChannelService> _logger;

    public TextChannelInviteService(
        DatabaseContext databaseContext, 
        ITextChannelService textChannelService, 
        IUserService userService, 
        ILogger<ITextChannelService> logger)
    {
        _databaseContext = databaseContext;
        _textChannelService = textChannelService;
        _userService = userService;
        _logger = logger;
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
            //TargetAccountId = target.AccountId,
            TextChannel = textChannel
        };

        _databaseContext.TextChannelInvites.Add(invite);
        _logger.LogInformation(
            $"Creating invite (Author: {authorGuid}, Target {targetGuid}, Channel: {textChannelGuid})");
        await _databaseContext.SaveChangesAsync();
    }

    public async Task AcceptInviteAsync(Guid inviteGuid)
    {
        var invite = await GetInviteAsync(inviteGuid);
        _logger.LogInformation($"Invite accepted! ({inviteGuid})");
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