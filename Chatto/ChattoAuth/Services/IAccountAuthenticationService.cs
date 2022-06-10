using ChattoAuth.Entities;
namespace ChattoAuth.Services;

public interface IAccountAuthenticationService
{
    Task<Account> AuthenticateAsync(string googleJwt);
}