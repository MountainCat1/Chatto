using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.Exceptions;

namespace Shared.Services;

public interface IMicroServiceAuthenticationService
{
    public Task<string> GetMicroserviceJWT();
    public Task<bool> ValidateMicroserviceJWT(string jwtToken);
}

public class MicroServiceAuthenticationService : IMicroServiceAuthenticationService
{
    private readonly MicroserviceSecuritySettings _securitySettings;

    public MicroServiceAuthenticationService(MicroserviceSecuritySettings securitySettings)
    {
        _securitySettings = securitySettings;
    }

    public async Task<string> GetMicroserviceJWT()
    {
        var claims = new List<Claim>()
        {
            new Claim(ClaimTypes.Name, _securitySettings.ServiceName),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_securitySettings.JwtKey));
        var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(_securitySettings.JwtExpireDays);
        var token = new JwtSecurityToken(_securitySettings.JwtIssuer,
            _securitySettings.JwtIssuer,
            claims,
            expires: expires,
            signingCredentials: cred);
        var tokenHandler = new JwtSecurityTokenHandler();
        return tokenHandler.WriteToken(token);
    }

    public async Task<bool> ValidateMicroserviceJWT(string jwtToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = _securitySettings.JwtKey;
        var validationParameters = new TokenValidationParameters()
        {
            ValidateLifetime = true, 
            ValidateAudience = true,
            ValidAudience = _securitySettings.JwtIssuer,
            ValidateIssuer = true,
            ValidIssuer = _securitySettings.JwtIssuer,

            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
        
        IPrincipal principal = tokenHandler.ValidateToken(jwtToken, validationParameters, out var validatedToken);
        // If principal is null, validation failed
        if (principal == null)
            throw new InvalidJwtException();
        
        var securityToken = (JwtSecurityToken)tokenHandler.ReadToken(jwtToken);
        var serviceName = int.Parse(securityToken.Claims.First(c => c.Type == ClaimTypes.Name).Value);

        return true;
    }
}