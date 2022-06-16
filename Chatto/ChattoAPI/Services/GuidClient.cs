using Shared.Extensions;

namespace Chatto.Services;

public interface IGuidClient
{
    Task<Guid> GetGuidAsync();
}

public class GuidClient : IGuidClient
{
    private readonly HttpClient _httpClient;
    
    private const string GetGuidRequestUri = "api/Guid";
    
    public GuidClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Guid> GetGuidAsync()
    {
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(GetGuidRequestUri, UriKind.Relative)
        };

        var response = await _httpClient.SendAsync(request);
        var guidText = await response.Content.GetTextAsync();
        return new Guid(guidText);
    }

}