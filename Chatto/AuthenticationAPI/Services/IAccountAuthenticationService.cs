using ChattoAuth.Entities;
namespace ChattoAuth.Services;

public interface IAccountAuthenticationService<TAccount, in TAuthenticationData> where TAccount : Account
{
    /// <summary>
    /// Authenticates account via authentication data, if authentication is successful returns connected account
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    Task<TAccount> AuthenticateAsync(HttpRequest request);

    Task RegisterAsync(TAuthenticationData authenticationData);
}