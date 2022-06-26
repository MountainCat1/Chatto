
using ChattoAuth.Exceptions;
using ChattoAuth.Infrastructure;
using ChattoAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;

namespace ChattoAuth.Services;

public interface IChattoAuthenticationService : IAccountAuthenticationService<ChattoAccount, ChattoAccountAuthenticationDataModel>
{
    // Intentionally empty
    Task<ChattoAccount> AuthenticateAsync(int accountId, string password);
}

public class ChattoAuthenticationService : IChattoAuthenticationService
{
    private readonly ILogger<IChattoAuthenticationService> _logger;
    private readonly DatabaseContext _databaseContext;
    private readonly IPasswordHasher<ChattoAccount> _passwordHasher;

    public ChattoAuthenticationService(
        ILogger<IChattoAuthenticationService> logger,
        DatabaseContext databaseContext, 
        IPasswordHasher<ChattoAccount> passwordHasher)
    {
        _logger = logger;
        _databaseContext = databaseContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<ChattoAccount> AuthenticateAsync(HttpRequest request)
    {
        _logger.LogInformation($"Authenticating http request with google...");
        
        request.Headers.TryGetValue("Authorization", out var authorizationHeader);
        var authorizationHeaderParts = authorizationHeader.ToString().Split('.');

        if (authorizationHeaderParts.Length < 2)
            throw new InvalidAuthenticationDataException("Authentication header should have at least 2 parts");
        if(!int.TryParse(authorizationHeaderParts[0], out int accountId))
            throw new InvalidAuthenticationDataException("First part must be an integer");
        
        var password = authorizationHeaderParts[1];

        return await AuthenticateAsync(accountId, password);
    }

    public async Task<ChattoAccount> AuthenticateAsync(int accountId, string password)
    {
        var account = await _databaseContext.Accounts
            .OfType<ChattoAccount>()
            .FirstOrDefaultAsync(x => x.Id == accountId);
        
        if(account == null)
            throw new InvalidAuthenticationDataException("Invalid account ID");

        
        var verificationResult = _passwordHasher.VerifyHashedPassword(
            account, 
            account.PasswordHash, 
            password);

        if (verificationResult == PasswordVerificationResult.Failed)
            throw new ForbidException("Wrong password");

        _logger.LogInformation($"Chatto Authentication successful!");
        
        return account;
    }
    
    public async Task<ChattoAccountAuthenticationDataModel> RegisterAsync(
        ChattoAccountAuthenticationDataModel authenticationData)
    {
        _logger.LogInformation($"Registering chatto account...");

        var newAccount = new ChattoAccount();
        var passwordHash = _passwordHasher.HashPassword(newAccount, authenticationData.Password);
        newAccount.PasswordHash = passwordHash;
        
        _databaseContext.Accounts.Add(newAccount);
        await _databaseContext.SaveChangesAsync();

        authenticationData.AccountId = newAccount.Id;
        
        return authenticationData;
    }
}