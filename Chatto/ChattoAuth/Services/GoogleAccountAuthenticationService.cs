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

public interface IGoogleAuthenticationService : IAccountAuthenticationService<GoogleAccount, GoogleAuthenticationData>
{
    /// <summary>
    /// Returns GoogleAccountData with google ID taken from google jwt
    /// </summary>
    /// <param name="googleJwt"></param>
    /// <returns></returns>
    public Task<GoogleAccount> CreateAccountEntity(string googleJwt);
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
    
    public async Task<GoogleAccount> CreateAccountEntity(string googleJwt)
    {
        return new GoogleAccount()
        {
            GoogleId = (await ValidateGoogleJwt(googleJwt)).Subject
        };
    }

    /// <summary>
    /// Validated google JWT token and return associated account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public async Task<GoogleAccount> Authenticate(HttpRequest request)
    {
        request.Headers.TryGetValue("Authorization", out var googleJwt);
        
        var jwtPayload = await ValidateGoogleJwt(googleJwt);

        var account = await _databaseContext.Accounts
            .OfType<GoogleAccount>()
            .FirstAsync(x => x.GoogleId == jwtPayload.Subject);

        return account;
    }

    public async Task Register(GoogleAuthenticationData authenticationData)
    {
        var googleId = (await ValidateGoogleJwt(authenticationData.Jwt)).Subject;

        var newAccount = new GoogleAccount()
        {
            GoogleId = googleId
        };

        await _databaseContext.Accounts.AddAsync(newAccount);
        await _databaseContext.SaveChangesAsync();
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

public class GoogleAuthenticationData
{
    public string Jwt { get; set; }
}