using System.Text.Json.Serialization;

namespace ChattoAuth.Configuration;
public class AuthenticationSettings
{
    public int JwtExpireDays { get; set; }
    public Google Google { get; set; }
    public Chatto Chatto { get; set; }
}
public class Chatto
{
    public string JwtKey { get; set; }
    public string JwtIssuer { get; set; }
}
public class Google
{
    [ConfigurationKeyName("client_id")]
    public string ClientId { get; set; }

    [ConfigurationKeyName("project_id")]
    public string ProjectId { get; set; }

    [ConfigurationKeyName("auth_uri")]
    public string AuthUri { get; set; }

    [ConfigurationKeyName("token_uri")]
    public string TokenUri { get; set; }

    [ConfigurationKeyName("auth_provider_x509_cert_url")]
    public string AuthProviderX509CertUrl { get; set; }

    [ConfigurationKeyName("client_secret")]
    public string ClientSecret { get; set; }

    [ConfigurationKeyName("redirect_uris")]
    public List<string> RedirectUris { get; set; }

    [ConfigurationKeyName("javascript_origins")]
    public List<string> JavascriptOrigins { get; set; }
}