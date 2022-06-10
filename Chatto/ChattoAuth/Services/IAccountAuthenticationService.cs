using ChattoAuth.Entities;
namespace ChattoAuth.Services;

public interface IAccountAuthenticationService
{
    Task<Account> Authenticate(string googleJwt);
}