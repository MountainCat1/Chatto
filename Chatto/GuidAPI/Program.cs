using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;

var builder = WebApplication.CreateBuilder(args);


// ========= CONFIGURATION
var configuration = builder.Configuration;

configuration.AddJsonFile("Secrets/SecuritySettings.json");
var securitySettings = new MicroserviceSecuritySettings();


// ========= SERVICES

var services = builder.Services;

services.AddControllers();
services.AddLogging();

services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)    
    .AddJwtBearer(options =>    
    {    
        options.TokenValidationParameters = new TokenValidationParameters    
        {    
            ValidateIssuer = true,    
            ValidateAudience = true,    
            ValidateLifetime = true,    
            ValidateIssuerSigningKey = true,    
            ValidIssuer = securitySettings.JwtIssuer,    
            ValidAudience = securitySettings.JwtIssuer,    
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securitySettings.JwtKey))    
        };    
    });

// ========= BUILD

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();