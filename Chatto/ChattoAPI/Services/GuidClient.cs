using Shared.Configuration;
using Shared.Extensions;
using Shared.Services;

namespace Chatto.Services;

public interface IGuidClient
{
    Task<Guid> GetGuidAsync();
}

public class GuidClient : IGuidClient
{
    private readonly HttpClient _httpClient;
    private readonly IMicroserviceAuthenticationService _securityService;

    private const string GetGuidRequestUri = "api/Guid";
    
    public GuidClient(HttpClient httpClient, IMicroserviceAuthenticationService securityService)
    {
        _httpClient = httpClient;
        _securityService = securityService;
    }

    public async Task<Guid> GetGuidAsync()
    {
        var securityToken = await _securityService.GetMicroserviceJWTAsync();
        
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(GetGuidRequestUri, UriKind.Relative),
            Headers = { {"Authorization", securityToken} }
        };

        var response = await _httpClient.SendAsync(request);
        var guidText = await response.Content.GetTextAsync();
        return new Guid(guidText);
    }

}