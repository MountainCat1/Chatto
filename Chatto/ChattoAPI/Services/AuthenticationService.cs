using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chatto.Infrastructure;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace Chatto.Services;

public interface IAuthenticationService
{
    Task<string> LoginUserGoogle(HttpRequest request);
    Task<string> RegisterUserGoogle(GoogleRegisterModel registerModel);
}

public class AuthenticationService : IAuthenticationService
{
    private readonly IAuthenticationClient _authenticationClient;

    private readonly IGuidClient _guidClient;
    private readonly DatabaseContext _databaseContext;


    public AuthenticationService(
        IAuthenticationClient authenticationClient, 
        DatabaseContext databaseContext, 
        IGuidClient guidClient)
    {
        _authenticationClient = authenticationClient;
        _databaseContext = databaseContext;
        _guidClient = guidClient;
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
        
        await _databaseContext.Users.AddAsync(newUser);
        await _databaseContext.SaveChangesAsync();
        
        return tokenString;
    }
}