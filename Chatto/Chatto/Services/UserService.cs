using System.Security.Claims;
using Chatto.Exceptions;
using Chatto.Extensions;
using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface IUserService
{
    Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal);
    Task<User> GetUserAsync(int accountId);
    Task<User> GetUserAsync(string username);
    Task<User> GetUserAsync(Guid guid);
    Task<IList<TextChannel>> GetUserTextChannelsAsync(Guid userGuid);
    Task<User> CreateUserAsync(string username, int accountId);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IGuidClient _guidClient;

    public UserService(DatabaseContext databaseContext, IGuidClient guidClient)
    {
        _databaseContext = databaseContext;
        _guidClient = guidClient;
    }

    public async Task<User> GetUserAsync(int accountId)
    {
        var entity = await _databaseContext.Users.FirstOrDefaultAsync(x => x.AccountId == accountId);

        if (entity == null)
            throw new NotFoundException("User not found");
        
        return entity;
    }

    public async Task<User> GetUserAsync(string username)
    {
        var entity = await _databaseContext.Users.FirstOrDefaultAsync(x => x.Username == username);

        if (entity == null)
            throw new NotFoundException("User not found");
        
        return entity;
    }

    public async Task<User> GetUserAsync(Guid guid)
    {
        var entity = await _databaseContext.Users.FindAsync(guid);
        
        if (entity == null)
            throw new NotFoundException("User not found");
        
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

    public async Task<User> CreateUserAsync(string username, int accountId)
    {
        if (_databaseContext.Users.Any(u => u.Username == username))
            throw new ArgumentException($"There already exists user with username: {username}");
        
        var newUser = new User()
        {
            Guid = await _guidClient.GetGuidAsync(),
            Username = username,
            AccountId = accountId,
            TextChannels = new List<TextChannel>()
        };
        _databaseContext.Add(newUser);
        await _databaseContext.SaveChangesAsync();
        return newUser;
    }

    public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal)
    {
        var claim = claimsPrincipal.FindFirst(u => u.Type == ClaimTypes.NameIdentifier);
        var userEntity = await GetUserAsync(int.Parse(claim.Value));
        return userEntity;
    }
}