using Chat.Domain.Entities;
using Chat.Infrastructe.ChatContext.ChatContextSeed;
using Chat.Infrastructe.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();
services.AddLogging();
SwaggerServicesExtention.AddSwaggerServices(builder.Services);


// Configure Servivce 

services.AddConfigurePresistanceService(builder.Configuration);
services.AddApplicationServiceExtension();
services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


// Configure Online Users 
services.AddSingleton<PresenceTracker>();// Using Disctionry To Check Each User

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod().SetIsOriginAllowed(orjan => true);
    });
});

var app = builder.Build();
using var scope = app.Services.CreateScope();
var Services = scope.ServiceProvider;
var loggerFactory = Services.GetRequiredService<ILoggerFactory>();
try
{
    var identityContext = Services.GetRequiredService<ApplicationDbContext>();
    await identityContext.Database.MigrateAsync();
    var roleManager = Services.GetRequiredService<RoleManager<IdentityRole>>();
    await IdentitySeed.CreateRolesAsync(roleManager);
    var userManager = Services.GetRequiredService<UserManager<AppUser>>();
    await IdentitySeed.SeedUserAsync(userManager);
}
catch (Exception ex)
{
    Log.Error(ex, ex.Message);
}


//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PresenceHub>(pattern: "hubs/presence");
app.MapHub<MessageHub>(pattern: "hubs/message");
app.Run();