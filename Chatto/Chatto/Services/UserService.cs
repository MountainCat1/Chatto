using System.Security.Claims;
using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface IUserService
{
    Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal);
    Task<User> GetUserAsync(int accountId);
    Task<User> GetUserAsync(Guid guid);
    Task<IList<TextChannel>> GetUserTextChannelsAsync(Guid userGuid);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;

    public UserService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext; }

    public async Task<User> GetUserAsync(int accountId)
    {
        var entity = await _databaseContext.Users.FirstAsync(x => x.AccountId == accountId);
        
        return entity;
    }
    
    public async Task<User> GetUserAsync(Guid guid)
    {
        var entity = await _databaseContext.Users.FindAsync(guid);
        
        return entity;
    }
    
    public async Task<IList<TextChannel>> GetUserTextChannelsAsync(Guid userGuid)
    {
        var user = await _databaseContext.Users
            .Include(user => user.TextChannels)
            .FirstAsync(user => user.Guid == userGuid);

        var textChannels = user.TextChannels;

        return textChannels;
    }

    public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.FindFirst(u => u.Type == ClaimTypes.NameIdentifier);
        var userEntity = await GetUserAsync(int.Parse(claim.Value));
        return userEntity;
    }
}