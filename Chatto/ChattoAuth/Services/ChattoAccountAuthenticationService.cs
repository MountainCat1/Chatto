using ChattoAuth.Entities;

namespace ChattoAuth.Services;

public interface IChattoAuthenticationService : IAccountAuthenticationService
{
    
}

public class ChattoAuthenticationService : IChattoAuthenticationService
{
    public async Task<Account> Authenticate(string googleJwt)
    {
        // TODO local authentication and stuff
        throw new NotImplementedException();
    }
}
