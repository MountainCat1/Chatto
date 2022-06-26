
using ChattoAuth.Infrastructure;

namespace ChattoAuth.Services;

public interface IAccountAuthenticationService<TAccount, TAuthenticationData> where TAccount : Account
{
    /// <summary>
    /// Authenticates account via authentication data, if authentication is successful returns connected account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<TAccount> AuthenticateAsync(HttpRequest request);

    Task<TAuthenticationData> RegisterAsync(TAuthenticationData authenticationData);
}