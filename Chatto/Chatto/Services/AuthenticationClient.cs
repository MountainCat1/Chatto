using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Chatto.Configuration;
using Chatto.Extensions;
using Chatto.Models;

namespace Chatto.Services;

public interface IAuthenticationClient
{
    Task<string> RegisterUserChattoAsync(string username, string password);
    Task<string> LoginUserChattoAsync(int accountId, string password);
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

    private const string LoginChattoApiUrl = "api/Account/LoginChatto";
    private const string RegisterChattoApiUrl = "api/Account/RegisterChatto";
    

    public AuthenticationClient(MicroservicesSettings microservicesSettings, HttpClient httpClient, ILogger<IAuthenticationClient> logger)
    {
        _microservicesSettings = microservicesSettings;
        _httpClient = httpClient;
        _logger = logger;
    }
    
    public async Task<string> RegisterUserChattoAsync(string username, string password)
    {
        var content = new ChattoRegisterModel()
        {
            AccountId = -1,
            Password = password,
            Username = username
        };
        
        var contentSerialized = new StringContent(JsonSerializer.Serialize(content));
        
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(RegisterChattoApiUrl, UriKind.Relative),
            Content = contentSerialized,
        };
        httpMessage.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        
        
        var response = await _httpClient.SendAsync(httpMessage);
        return await response.Content.GetTextAsync();
    }
    
    public async Task<string> LoginUserChattoAsync(int accountId, string password)
    {
        var httpMessage = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(LoginChattoApiUrl, UriKind.Relative),
            Headers = { { "Authorization", $"{accountId}.{password}" } }
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