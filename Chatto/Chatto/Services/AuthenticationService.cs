using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chatto.Extensions;
using Chatto.Infrastructure;
using Chatto.Models;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

namespace Chatto.Services;

public interface IAuthenticationService
{
    Task<string> LoginUserGoogle(HttpRequest request);
    Task<string> RegisterUserGoogle(GoogleRegisterModel registerModel);
    Task<string> RegisterUserChatto(ChattoRegisterModel registerModel);
    Task<string> LoginUserChatto(HttpRequest request);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationClient _authenticationClient;

    private readonly IGuidClient _guidClient;
    private readonly DatabaseContext _databaseContext;
    private readonly IUserService _userService;


    public AuthenticationService(
        IAuthenticationClient authenticationClient, 
        DatabaseContext databaseContext, 
        IGuidClient guidClient, IUserService userService)
    {
        _authenticationClient = authenticationClient;
        _databaseContext = databaseContext;
        _guidClient = guidClient;
        _userService = userService;
    }

    public async Task<string> LoginUserGoogle(HttpRequest request)
    {
        var token = await _authenticationClient.LoginUserGoogleAsync(request.Headers["Authorization"]);

        return token;
    }

    public async Task<string> RegisterUserGoogle(GoogleRegisterModel registerModel)
    {
        var tokenString = await _authenticationClient.RegisterUserGoogleAsync(registerModel.GoogleJwt);
        
        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(tokenString);
        var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);

        
        // If user with associated account ID already exists, dont create new one
        // just send back auth token
        if (await _databaseContext.Users.AnyAsync(x => x.AccountId == accountId))
            return tokenString;
        
        var newUser = new User()
        {
            Guid = await _guidClient.GetGuidAsync(),
            AccountId = accountId,
            Username = registerModel.Username
        };
        
        await _userService.CreateUserAsync(registerModel.Username, accountId);
        
        return tokenString;
    }

    public async Task<string> RegisterUserChatto(ChattoRegisterModel registerModel)
    {
        var tokenString = await _authenticationClient.RegisterUserChattoAsync(
            registerModel.Username, 
            registerModel.Password);

        var jwtHandler = new JwtSecurityTokenHandler();
        var jwtToken = jwtHandler.ReadJwtToken(tokenString);
        var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value);
        
        // If user with associated account ID already exists, dont create new one
        // just send back auth token
        if (await _databaseContext.Users.AnyAsync(x => x.AccountId == accountId))
            return tokenString;
        
        var newUser = new User()
        {
            Guid = await _guidClient.GetGuidAsync(),
            AccountId = accountId,
            Username = registerModel.Username
        };

        await _userService.CreateUserAsync(registerModel.Username, accountId);

        return tokenString;
    }

    public async Task<string> LoginUserChatto(HttpRequest request)
    {
        request.Headers.TryGetValue("Authorization", out var authorizationHeader);
        var authorizationHeaderParts = authorizationHeader.ToString().Split('.');

        if (authorizationHeaderParts.Length < 2)
            throw new InvalidAuthenticationDataException("Authentication header should have at least 2 parts");

        var username = authorizationHeaderParts[0];
        var user = await _userService.GetUserAsync(username);
        
        var password = authorizationHeaderParts[1];
        
        var token = await _authenticationClient.LoginUserChattoAsync(user.AccountId, password);

        return token;
    }
}