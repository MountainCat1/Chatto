using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chatto.Configuration;
using Shared.Extensions;

namespace Chatto.Services;

public interface IAuthenticationClient
{
    Task<string> RegisterUserChattoAsync(string username, string hashedPassword);
    Task<string> LoginUserChattoAsync(string username, string hashedPassword);
    Task<string> RegisterUserGoogleAsync(string googleToken);
    Task<string> LoginUserGoogleAsync(string googleToken);
}

public class AuthenticationClient : HttpClient, IAuthenticationClient
{
    private readonly MicroservicesSettings _microservicesSettings;
    private readonly ILogger<IAuthenticationClient> _logger;
    
    private readonly HttpClient _httpClient;

    private const string LoginGoogleApiUrl = "api/Account/LoginGoogle";
    private const string RegisterGoogleApiUrl = "api/Account/RegisterGoogle";

    private const string LoginChattoApiUrl = "api/Account/LoginGoogle";
    private const string RegisterChattoApiUrl = "api/Account/LoginGoogle";
    

    public AuthenticationClient(MicroservicesSettings microservicesSettings, HttpClient httpClient, ILogger<IAuthenticationClient> logger)
    {
        _microservicesSettings = microservicesSettings;
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<string> RegisterUserChattoAsync(string username, string hashedPassword)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RegisterChattoApiUrl, UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(new { username, hashedPassword }))
        };
        var response = await _httpClient.SendAsync(httpMessage);
        return await response.Content.GetTextAsync();
    }
    
    public async Task<string> LoginUserChattoAsync(string username, string hashedPassword)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(LoginChattoApiUrl, UriKind.Relative),
            Content = new StringContent(JsonSerializer.Serialize(new { username, hashedPassword }))
        };
        var response = await _httpClient.SendAsync(httpMessage);
        
        if(response.StatusCode != HttpStatusCode.OK)
            _logger.LogError("Response has not returned 200 OK");
        
        return await response.Content.GetTextAsync();
    }

    public async Task<string> RegisterUserGoogleAsync(string googleToken)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RegisterGoogleApiUrl, UriKind.Relative),
            Headers = { { "Authorization", $"Bearer {googleToken}" } }
        };
        var response = await _httpClient.SendAsync(httpMessage);
        
        if(response.StatusCode != HttpStatusCode.OK)
            _logger.LogError("Response has not returned 200 OK");
        
        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }

    public async Task<string> LoginUserGoogleAsync(string googleToken)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(LoginGoogleApiUrl, UriKind.Relative),
            Headers = { { "Authorization", $"Bearer {googleToken}" } }
        };
        var response = await _httpClient.SendAsync(httpMessage);
        
        if(response.StatusCode != HttpStatusCode.OK)
            _logger.LogError("Response has not returned 200 OK");
        
        using (var reader = new StreamReader(await response.Content.ReadAsStreamAsync(), Encoding.UTF8))
        {
            return await reader.ReadToEndAsync();
        }
    }
}