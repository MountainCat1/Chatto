using System.Text;
using System.Text.Json.Serialization;
using Chatto.Configuration;
using Chatto.Controllers;
using Chatto.Infrastructure;
using Chatto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Shared.Configuration;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;
var services = builder.Services;
// ========= CONFIGURATION  =========


configuration.AddJsonFile("microservicesSettings.json");
var microservicesSettings = new MicroservicesSettings();
configuration.GetSection("MicroservicesSettings").Bind(microservicesSettings);
services.AddSingleton(microservicesSettings);

configuration.AddJsonFile("Secrets/Authentication.json");
var authenticationSettings = new AuthenticationSettings();
configuration.GetSection("AuthenticationSettings").Bind(authenticationSettings);
services.AddSingleton(authenticationSettings);

configuration.AddJsonFile("Secrets/SecuritySettings.json");
var securitySettings = new MicroserviceSecuritySettings();
configuration.GetSection("SecuritySettings").Bind(securitySettings);
services.AddSingleton(securitySettings);

// ======== SERVICES


services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
services.AddAuthentication(option =>
    {
        option.DefaultAuthenticateScheme = "Bearer";
        option.DefaultScheme = "Bearer";
        option.DefaultChallengeScheme = "Bearer";
    }
).AddJwtBearer(cfg =>
{
    cfg.RequireHttpsMetadata = true;
    cfg.SaveToken = true;
    cfg.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = authenticationSettings.JwtIssuer,
        ValidAudience = authenticationSettings.JwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
    };
});
services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
});

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy =>
        {
            policy.WithOrigins(configuration.GetSection("Cors:AllowedDomains").Get<string[]>())
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
services.AddDbContext<DatabaseContext>((options) =>
{
    options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection"));
});

services.AddSwaggerGen();


services.AddScoped<IMicroserviceAuthenticationService, MicroserviceAuthenticationService>();

services.AddHttpClient<IAuthenticationClient, AuthenticationClient>(client =>
{
    client.BaseAddress = new Uri(microservicesSettings.AuthenticationSettings.Url);
});
services.AddHttpClient<IGuidClient, GuidClient>(client =>
{
    client.BaseAddress = new Uri(microservicesSettings.GuidSettings.Url);
});

services.AddScoped<IAuthenticationService, AuthenticationService>();
services.AddScoped<DatabaseSeeder>();
services.AddScoped<IUserService, UserService>();
services.AddScoped<ITextChannelService, TextChannelService>();


// ======== APP

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
    
    var seeder = new DatabaseSeeder(services.BuildServiceProvider().GetService<DatabaseContext>());
    seeder.Seed(false);
}

app.UseHttpsRedirection();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseRouting();
app.UseCors();

app.UseAuthentication();
app.UseAuthorization();

app.Run();