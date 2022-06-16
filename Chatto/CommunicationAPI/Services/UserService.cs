using AutoMapper;
using Chatto.Infrastructure;
using Shared.Models;

namespace CommunicationAPI.Services;

public interface IUserService
{
    Task CreateUserAsync(CreateUserModel createModel);
}

public class UserService : IUserService
{
    private readonly DatabaseContext _databaseContext;
    private readonly IMapper _mapper;
    private readonly Logger<IUserService> _logger;

    public UserService(
        DatabaseContext databaseContext, 
        IMapper mapper)
    {
        _databaseContext = databaseContext;
        _mapper = mapper;
    }

    public async Task CreateUserAsync(CreateUserModel createModel)
    {
        _logger.LogInformation($"Creating new user... {createModel.Username}");
        var entity = _mapper.Map<User>(createModel);

        await _databaseContext.Users.AddAsync(entity);
        await _databaseContext.SaveChangesAsync();
    }
}