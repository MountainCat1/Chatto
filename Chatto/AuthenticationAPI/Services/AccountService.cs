using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using ChattoAuth.Configuration;
using ChattoAuth.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Shared.Exceptions;

namespace ChattoAuth.Services;

public interface IAccountService
{
    /// <summary>
    /// Returns user with associated ID
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    Task<Account> GetAccountAsync(int accountId);

    /// <summary>
    /// Generates JWT containing ChattoAPI signature and account ID 
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    Task<string> GetAccountJwtAsync(Account account);

    /// <summary>
    /// Validates JWT token, if validation is successful returns account ID
    /// </summary>
    /// <param name="authToken"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    bool AuthenticateChattoJwtToken(string authToken, out int accountId);
    /// <summary>
    /// Validates JWT token, if validation is successful returns account entity
    /// </summary>
    /// <param name="authToken"></param>
    /// <param name="account"></param>
    /// <returns></returns>
    bool AuthenticateChattoJwtToken(string authToken, out Account account);
}

public class AccountService : IAccountService
{
    private readonly AuthenticationSettings _authenticationSettings;
    private readonly DatabaseContext _databaseContext;
    private readonly ILogger<IAccountService> _logger;

    private readonly IGoogleAuthenticationService _googleAuthenticationService;

    public AccountService(
        AuthenticationSettings authenticationSettings, 
        DatabaseContext databaseContext, 
        IGoogleAuthenticationService googleAuthenticationService, 
        ILogger<IAccountService> logger)
    {
        _authenticationSettings = authenticationSettings;
        _databaseContext = databaseContext;
        _googleAuthenticationService = googleAuthenticationService;
        _logger = logger;
    }

    /// <summary>
    /// Returns user with associated ID
    /// </summary>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public async Task<Account> GetAccountAsync(int accountId)
    {
        return await _databaseContext.Accounts.FirstAsync(x => x.Id == accountId);
    }

    /// <summary>
    /// Generates JWT containing ChattoAPI signature and account ID 
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public async Task<string> GetAccountJwtAsync(Account account)
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.Chatto.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);
        var token = new JwtSecurityToken(_authenticationSettings.Chatto.JwtIssuer,
            _authenticationSettings.Chatto.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: cred);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    /// <summary>
    /// Validates JWT token, if validation is successful returns account ID
    /// </summary>
    /// <param name="authToken"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    public bool AuthenticateChattoJwtToken(string authToken, out int accountId)
    {
        _logger.LogInformation($"Authenticating chatto jwt... {authToken}");
        accountId = -1;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _authenticationSettings.Chatto.JwtKey;
        var validationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = true, 
            ValidateAudience = true,
            ValidAudience = _authenticationSettings.Chatto.JwtIssuer,
            ValidateIssuer = true,
            ValidIssuer = _authenticationSettings.Chatto.JwtIssuer,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
        
        IPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out var validatedToken);
        // If principal is null, validation failed
        if (principal == null)
            throw new InvalidJwtException();
        
        var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(authToken);
        accountId = int.Parse(securityToken.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
            
        _logger.LogInformation($"Authentication successful! (Account ID: {accountId})");
        
        return true;
    }
    
    /// <summary>
    /// Validates JWT token, if validation is successful returns account entity
    /// </summary>
    /// <param name="authToken"></param>
    /// <param name="account"></param>
    /// <returns></returns>
    public bool AuthenticateChattoJwtToken(string authToken, out Account account)
    {
        var validated = AuthenticateChattoJwtToken(authToken, out int accountId);
        account = _databaseContext.Accounts.First(x => x.Id == accountId);
        return validated;
    }
}

