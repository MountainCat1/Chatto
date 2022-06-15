using Chatto.Models;
using Microsoft.AspNetCore.DataProtection;

namespace Chatto.Services;

public interface IUserService
{
    Task<string> LoginUserGoogle(HttpRequest request);
    Task<string> RegisterUserGoogle(HttpRequest request);
}

public class UserService : IUserService
{
    private readonly IAuthenticationClient _authenticationClient;


    public UserService(IAuthenticationClient authenticationClient)
    {
        _authenticationClient = authenticationClient;
    }

    public async Task<string> LoginUserGoogle(HttpRequest request)
    {
        var token = await _authenticationClient.LoginUserGoogle(request.Headers["Authorization"]);

        return token;
    }

    public async Task<string> RegisterUserGoogle(HttpRequest request)
    {
        var token = await _authenticationClient.RegisterUserGoogle(request.Headers["Authorization"]);

        return token;
    }
}