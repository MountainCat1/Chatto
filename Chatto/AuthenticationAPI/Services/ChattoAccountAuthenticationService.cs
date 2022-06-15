
using ChattoAuth.Infrastructure;

namespace ChattoAuth.Services;

public interface IChattoAuthenticationService : IAccountAuthenticationService<ChattoAccount, ChattoAccountData>
{
    
}

public class ChattoAuthenticationService : IChattoAuthenticationService
{
    public async Task<ChattoAccount> AuthenticateAsync(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task RegisterAsync(ChattoAccountData authenticationData)
    {
        throw new NotImplementedException();
    }
}

public class ChattoAccountData
{
    public string PasswordHash { get; set; }
}
