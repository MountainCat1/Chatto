using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface IUserService
{
    Task<User> GetUserAsync(int accountId);
    Task<ICollection<TextChannel>> GetUserTextChannels(Guid userGuid);
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

    public async Task<ICollection<TextChannel>> GetUserTextChannels(Guid userGuid)
    {
        var user = await _databaseContext.Users
            .Include(x => x.TextChannels)
            .FirstAsync(x => x.Guid == userGuid);

        return user.TextChannels;
    }
}