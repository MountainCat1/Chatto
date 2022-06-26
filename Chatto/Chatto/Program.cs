using System.Text;
using System.Text.Json.Serialization;
using Chatto;
using Chatto.AuthorizationHandlers;
using Chatto.Configuration;
using Chatto.Controllers;
using Chatto.Extensions;
using Chatto.Infrastructure;
using Chatto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;
var services = builder.Services;
// ========= CONFIGURATION  =========


var microservicesSettings =
    configuration.AddConfigurationFile<MicroservicesSettings>("microservicesSettings.json",
        "MicroservicesSettings", services);

var authenticationSettings =
    configuration.AddConfigurationFile<AuthenticationSettings>("Secrets/Authentication.json",
        "AuthenticationSettings", services);

var securitySettings =
    configuration.AddConfigurationFile<MicroserviceSecuritySettings>("Secrets/SecuritySettings.json",
        "SecuritySettings", services);

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
    options.AddPolicy(AuthorizationPolicies.Authenticated, policy => policy
        .RequireAuthenticatedUser());
    
    options.DefaultPolicy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    
    options.AddPolicy(Operations.SendMessage, policy => policy
        .Requirements.Add(new IsAMemberRequirement()));
    
    options.AddPolicy(Operations.View, policy => policy
        .Requirements.Add(new IsAMemberRequirement()));
    
    options.AddPolicy(Operations.InviteNewMembers, policy => policy
        .Requirements.Add(new IsAMemberRequirement()));
});
services.AddSingleton<IAuthorizationHandler, TextChannelAuthorizationHandler>();


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
services.AddAutoMapper(typeof(Program).Assembly);
services.AddLogging();

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
services.AddScoped<ITextChannelInviteService, TextChannelInviteService>();


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