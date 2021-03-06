using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using ChattoAuth.Configuration;
using ChattoAuth.Infrastructure;
using ChattoAuth.Models;
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace ChattoAuth.Services;

public interface IGoogleAuthenticationService : IAccountAuthenticationService<GoogleAccount, GoogleAuthenticationData>
{
    // Intentionally empty
}

public class GoogleAuthenticationService : IGoogleAuthenticationService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<IGoogleAuthenticationService> _logger;

    public GoogleAuthenticationService(
        AuthenticationSettings authenticationSettings, 
        DatabaseContext databaseContext,
        ILogger<IGoogleAuthenticationService> logger)
    {
        _authenticationSettings = authenticationSettings;
        _databaseContext = databaseContext;
        _logger = logger;
    }

    /// <summary>
    /// Validated google JWT token and return associated account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<GoogleAccount> AuthenticateAsync(HttpRequest request)
    {
        _logger.LogInformation($"Authenticating http request with google...");
        
        request.Headers.TryGetValue("Authorization", out var googleJwt);
        
        var jwtPayload = await ValidateGoogleJwt(googleJwt);

        var account = await _databaseContext.Accounts
            .OfType<GoogleAccount>()
            .FirstAsync(x => x.GoogleId == jwtPayload.Subject);

        _logger.LogInformation($"Google Authentication successful!");
        
        return account;
    }

    public async Task<GoogleAuthenticationData> RegisterAsync(GoogleAuthenticationData authenticationData)
    {
        _logger.LogInformation($"Registering account... {authenticationData.Jwt}");

        var googleId = (await ValidateGoogleJwt(authenticationData.Jwt)).Subject;

        
        // If account with associated google ID exists, don't create a new one
        if (await _databaseContext.Accounts
                .OfType<GoogleAccount>()
                .AnyAsync(x => x.GoogleId == googleId))
        {
            return null;
        }
        
        var newAccount = new GoogleAccount()
        {
            GoogleId = googleId
        };

        await _databaseContext.Accounts.AddAsync(newAccount);
        await _databaseContext.SaveChangesAsync();
        
        _logger.LogInformation($"Account registered!");
        return authenticationData;
    }

    /// <summary>
    /// Validates google JWT and its return payload
    /// </summary>
    /// <param name="jwt"></param>
    /// <returns></returns>
    private async Task<GoogleJsonWebSignature.Payload> ValidateGoogleJwt(string jwt)
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