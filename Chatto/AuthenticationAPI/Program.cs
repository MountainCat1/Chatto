using System.Configuration;
using System.Text.Json.Serialization;
using ChattoAuth.Configuration;
using ChattoAuth.Infrastructure;
using ChattoAuth.Services;
using ChattoAuth.Validators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

// ========= BUILDER  =========
var builder = WebApplication.CreateBuilder(args);

// ========= CONFIGURATION  =========
var configuration = builder.Configuration;


configuration.AddJsonFile("Secrets/Authentication.json");
var authenticationSettings = new AuthenticationSettings();
configuration.GetSection("AuthenticationSettings").Bind(authenticationSettings);

// ========= SERVICES  =========
var services = builder.Services;
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

services.AddSwaggerGen();
services.AddAutoMapper(typeof(Program).Assembly);

services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy  =>
        {
            policy.WithOrigins(configuration.GetSection("Cors:AllowedDomains").Get<string[]>())
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

services.AddDbContext<DatabaseContext>(
    options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("DatabaseConnection"));
    });

services.AddSingleton<AuthenticationSettings>(authenticationSettings);
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IGoogleAuthenticationService, GoogleAuthenticationService>();



// Authentication
services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddGoogle(googleOptions =>
    {
        googleOptions.ClientId = authenticationSettings.Google.ClientId;
        googleOptions.ClientSecret = authenticationSettings.Google.ClientSecret;
    })
    .AddJwtBearer(o =>
    {
        o.SecurityTokenValidators.Clear();
        o.SecurityTokenValidators.Add(new GoogleTokenValidator(authenticationSettings.Google.ClientId));
    });

// ========= RUN APP  =========
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