var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;

services.AddControllers();

var app = builder.Build();
app.UseRouting();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();