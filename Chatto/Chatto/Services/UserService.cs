﻿using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Chatto.Infrastructure;
using Chatto.Models;
using Microsoft.AspNetCore.DataProtection;
using Shared.Models;

namespace Chatto.Services;

public interface IUserService
{
    Task<string> LoginUserGoogle(HttpRequest request);
    Task<string> RegisterUserGoogle(GoogleRegisterModel registerModel);
}

public class UserService : IUserService
{
    private readonly IAuthenticationClient _authenticationClient;
    private readonly IGuidClient _guidClient;
    private readonly DatabaseContext _databaseContext;


    public UserService(IAuthenticationClient authenticationClient, DatabaseContext databaseContext, IGuidClient guidClient)
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

        var newUser = new User()
        {
            Guid = await _guidClient.GetGuid(),
            AccountId = int.Parse(jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value),
            Username = registerModel.Username
        };

        await _databaseContext.Users.AddAsync(newUser);
        await _databaseContext.SaveChangesAsync();
        
        return tokenString;
    }
}