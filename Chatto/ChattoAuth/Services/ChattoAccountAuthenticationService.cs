using ChattoAuth.Entities;

namespace ChattoAuth.Services;

public interface IChattoAuthenticationService : IAccountAuthenticationService<ChattoAccount, ChattoAccountData>
{
    
}

public class ChattoAuthenticationService : IChattoAuthenticationService
{
    public async Task<ChattoAccount> Authenticate(HttpRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task Register(ChattoAccountData authenticationData)
    {
        throw new NotImplementedException();
    }
}

public class ChattoAccountData
{
    public string PasswordHash { get; set; }
}
