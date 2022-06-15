using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chatto.Configuration;
using Chatto.Models;

namespace Chatto.Services;

public interface IAuthenticationClient
{
    Task<string> RegisterUserChatto(string username, string hashedPassword);
    Task<string> LoginUserChatto(string username, string hashedPassword);
    Task<string> RegisterUserGoogle(string googleToken);
    Task<string> LoginUserGoogle(string googleToken);
}

public class AuthenticationClient : IAuthenticationClient
{
    private readonly MicroservicesSettings _microservicesSettings;

    private readonly HttpClient _httpClient;

    private const string LoginGoogleApiUrl = "api/Account/LoginGoogle";
    private const string RegisterGoogleApiUrl = "api/Account/RegisterGoogle";

    private const string LoginChattoApiUrl = "api/Account/LoginGoogle";
    private const string RegisterChattoApiUrl = "api/Account/LoginGoogle";

    public AuthenticationClient(MicroservicesSettings microservicesSettings)
    {
        _microservicesSettings = microservicesSettings;
        _httpClient = new HttpClient()
        {
            BaseAddress = new Uri(microservicesSettings.AuthenticationSettings.Url)
        };
    }
    
    public async Task<string> RegisterUserChatto(string username, string hashedPassword)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RegisterChattoApiUrl, UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(new { username, hashedPassword }))
        };
        var response = await _httpClient.SendAsync(httpMessage);
        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }
    
    public async Task<string> LoginUserChatto(string username, string hashedPassword)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(LoginChattoApiUrl, UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(new { username, hashedPassword }))
        };
        var response = await _httpClient.SendAsync(httpMessage);
        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }

    public async Task<string> RegisterUserGoogle(string googleToken)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RegisterGoogleApiUrl, UriKind.Relative),
            Headers = { { "Authorization", $"Bearer {googleToken}" } }
        };
        var response = await _httpClient.SendAsync(httpMessage);
        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }

    public async Task<string> LoginUserGoogle(string googleToken)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(LoginGoogleApiUrl, UriKind.Relative),
            Headers = { { "Authorization", $"Bearer {googleToken}" } }
        };
        var response = await _httpClient.SendAsync(httpMessage);
        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }
}