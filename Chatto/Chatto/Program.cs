using System.Text.Json.Serialization;
using Chatto.Configuration;
using Chatto.Services;

var builder = WebApplication.CreateBuilder(args);


// ========= CONFIGURATION  =========
var configuration = builder.Configuration;

configuration.AddJsonFile("microservicesSettings.json");
var microservicesSettings = new MicroservicesSettings();
configuration.GetSection("MicroservicesSettings").Bind(microservicesSettings);

// ======== SERVICES
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
            policy.WithOrigins(configuration.GetSection("Cors:AllowedDomains").Get<string[]>())
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

services.AddSwaggerGen();

services.AddSingleton<MicroservicesSettings>(microservicesSettings);
services.AddHttpClient<IAuthenticationClient, AuthenticationClient>(client =>
{
    client.BaseAddress = new Uri(microservicesSettings.AuthenticationSettings.Url);
});
services.AddHttpClient<IGuidClient, GuidClient>(client =>
{
    client.BaseAddress = new Uri(microservicesSettings.GuidSettings.Url);
});

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
