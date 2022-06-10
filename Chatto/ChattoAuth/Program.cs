using System.Configuration;
using System.Text.Json.Serialization;
using ChattoAuth.Configuration;
using ChattoAuth.Entities;
using ChattoAuth.Services;
using ChattoAuth.Validators;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;

// ========= BUILDER  =========
var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("Secrets/Authentication.json");
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

// ========= CONFIGURATION  =========
var configuration = builder.Configuration;

var authenticationSettings = new AuthenticationSettings();
configuration.GetSection("AuthenticationSettings").Bind(authenticationSettings);

// ========= SERVICES  =========
var services = builder.Services;
services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});
services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policy  =>
        {
            policy.WithOrigins(configuration.GetSection("FrontEnd:Url").Value)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
services.AddAutoMapper(typeof(Program).Assembly);
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
        //o.SecurityTokenValidators.Clear();
        //o.SecurityTokenValidators.Add(new GoogleTokenValidator(authenticationSettings.Google.ClientId));
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
app.UseAuthentication();
app.UseCors();

app.Run();