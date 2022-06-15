using Chatto.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Chatto.Services;

public interface IUserService
{
    Task<string> GetUser(HttpRequest request);
}

public class UserService : IUserService
{
    private readonly IAuthenticationClient _authenticationClient;


    public UserService(IAuthenticationClient authenticationClient)
    {
        _authenticationClient = authenticationClient;
    }

    public async Task<string> GetUser(HttpRequest request)
    {
        var token = await _authenticationClient.LoginUserGoogle(request.Headers["Authorization"]);

        return token;
    }
}