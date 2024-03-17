using Chat.Infrastructe.ExtensionMethods;
using Chat.Application.ExtensionMethods;
using Microsoft.OpenApi.Models;
using Chat.API.signalR;
using System.Text.Json.Serialization;

 var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.AddControllers();
services.AddLogging();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen(setupAction =>
{
    setupAction.SwaggerDoc("v1", new OpenApiInfo { Title = "Chat App", Version = "v1" });

    setupAction.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    });

    setupAction.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    // configure xml 
    var xmlFile = "ChatApp.Api.xml"; 
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    setupAction.IncludeXmlComments(xmlPath);
});




// Configure Servivce 

services.AddConfigurePresistanceService(builder.Configuration);
services.AddApplicationServiceExtension();
services.AddSignalR().AddJsonProtocol(options =>
{
    options.PayloadSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
});


// Configure Online Users 
services.AddSingleton<PresenceTracker>();// Using Disctionry To Check Each User



services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", builder =>
    {
        builder.AllowAnyHeader().AllowCredentials().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://localhost:4200");

    });
});

var app = builder.Build();
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI();
//}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("CorsPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<PresenceHub>(pattern: "hubs/presence");
app.MapHub<MessageHub>(pattern: "hubs/message");
ConfigurePresistanceService.ConfigureMiddleWare(app);
app.Run();