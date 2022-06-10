using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using ChattoAuth.Configuration;
using ChattoAuth.Entities;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using InvalidJwtException = ChattoAuth.Exceptions.InvalidJwtException;

namespace ChattoAuth.Services;

public interface IGoogleAuthenticationService : IAccountAuthenticationService
{
}

public class GoogleAuthenticationService : IGoogleAuthenticationService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly DatabaseContext _databaseContext;

    public GoogleAuthenticationService(
        AuthenticationSettings authenticationSettings, 
        DatabaseContext databaseContext)
    {
        _authenticationSettings = authenticationSettings;
        _databaseContext = databaseContext;
    }

    /// <summary>
    /// Validated google JWT token and return associated account
    /// </summary>
    /// <param name="googleJwt"></param>
    /// <returns></returns>
    public async Task<Account> AuthenticateAsync(string googleJwt)
    {
        var jwtPayload = await ValidateGoogleJwtAsync(googleJwt);

        var account = await _databaseContext.Accounts
            .OfType<GoogleAccount>()
            .FirstAsync(x => x.GoogleId == jwtPayload.Subject);

        return account;
    }
    
    /// <summary>
    /// Validates google JWT and its return payload
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleJwtAsync(string jwt)
    {
        if (jwt.Contains(' '))
            jwt = jwt.Split(' ').Last();

        try
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(
                jwt, 
                new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience =  new[] { _authenticationSettings.Google.ClientId }
                }
            );
            return payload;
        }
        catch (Exception e)
        {
            throw;
        }
    }
}