
using ChattoAuth.Infrastructure;

namespace ChattoAuth.Services;

public interface IChattoAuthenticationService : IAccountAuthenticationService<ChattoAccount, ChattoAccountData>
{
    
}

public class ChattoAuthenticationService : IChattoAuthenticationService
{
    private readonly ILogger<IChattoAuthenticationService> _logger;

    public ChattoAuthenticationService(ILogger<IChattoAuthenticationService> logger)
    {
        _logger = logger;
    }

    public async Task<ChattoAccount> AuthenticateAsync(HttpRequest request)
    {
        _logger.LogError($"Chatto authentication has not been implemented yet!");
        throw new NotImplementedException();
    }

    public async Task RegisterAsync(ChattoAccountData authenticationData)
    {
        _logger.LogError($"Chatto authentication has not been implemented yet!");
        throw new NotImplementedException();
    }
}

public class ChattoAccountData
{
    public string PasswordHash { get; set; }
}
