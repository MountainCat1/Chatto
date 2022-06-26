namespace Chatto.Configuration;

public class MicroserviceSecuritySettings
{
    public string JwtIssuer { get; set; }
    public double JwtExpireDays { get; set; }
    public string JwtKey { get; set; }
    public string ServiceName { get; set; }
}