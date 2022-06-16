using Chatto.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Controllers;

public interface IUserService
{
    Task<User> GetUserAsync(int accountId);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;

    public UserService(DatabaseContext databaseContext)
    {
        _databaseContext = databaseContext;
    }

    public async Task<User> GetUserAsync(int accountId)
    {
        var entity = await _databaseContext.Users.FirstAsync(x => x.AccountId == accountId);
        
        return entity;
    }
}