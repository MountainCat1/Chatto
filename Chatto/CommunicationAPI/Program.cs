using System.IdentityModel.Tokens.Jwt;
using System.Text.Json.Serialization;
using CommunicationAPI.Infrastructure;
using CommunicationAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Shared.Configuration;
using Shared.Services;

var builder = WebApplication.CreateBuilder(args);

// ========= CONFIGURATION  =========
var configuration = builder.Configuration;

configuration.AddJsonFile("Secrets/SecuritySettings.json");
var securitySettings = new MicroserviceSecuritySettings();
configuration.GetSection("SecuritySettings").Bind(securitySettings);

// ======== SERVICES
var services = builder.Services;

services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
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
services.AddLogging();
services.AddAuthentication()
    .AddJwtBearer(jwtBearerOptions =>
    {
        jwtBearerOptions.SecurityTokenValidators.Clear();
        jwtBearerOptions.SecurityTokenValidators.Add(new JwtSecurityTokenHandler());
    });
/*services.AddAuthorization(options =>
{
    options.DefaultPolicy = new AuthorizationPolicy("Default Policy");
});*/


services.AddAutoMapper(typeof(Program).Assembly);
services.AddSwaggerGen();

services.Configure<MicroserviceSecuritySettings>(
    options => configuration.GetSection("SecuritySettings").Bind(options));


services.AddScoped<IUserService, UserService>();
services.AddScoped<IMicroserviceAuthenticationService, MicroserviceAuthenticationService>();

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

    var dbContext = services.BuildServiceProvider().GetService<DatabaseContext>();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();