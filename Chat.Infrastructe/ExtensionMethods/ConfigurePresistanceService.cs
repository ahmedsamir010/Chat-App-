using Chat.Application.Helpers;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Chat.Infrastructe.ChatContext.ChatContextSeed;
using Chat.Infrastructe.Data;
using Chat.Infrastructe.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
namespace Chat.Infrastructe.ExtensionMethods
{
    public static class ConfigurePresistanceService
    {
        public static IServiceCollection AddConfigurePresistanceService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            // Configure
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped(typeof(IMessageRepository), typeof(MessageRepository));
            services.AddScoped(typeof(IUserRepository), typeof(UserRespository));
            services.AddScoped(typeof(ILikeRepository), typeof(LikeRepository));
            services.AddScoped<IUserValidator<AppUser>, CustomUserValidator<AppUser>>();
            // Configure Token 
            services.AddScoped<ITokenService, TokenService>();
            // Config identity
            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })

                      .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders()
                    .AddUserValidator<CustomUserValidator<AppUser>>();
            services.AddMemoryCache();
            services.AddAuthentication(option =>
            {
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                    .AddJwtBearer(options =>
                    {
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            ValidateIssuer = true,
                            ValidateAudience = false,
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Token:Key"])),
                            ValidIssuer = configuration["Token:Issuer"],
                            NameClaimType = ClaimTypes.NameIdentifier,

                        };
                        options.Events = new JwtBearerEvents()
                        {
                            OnMessageReceived = context =>
                            {
                                var accessToken = context.Request.Query["access_token"];
                                var path = context.HttpContext.Request.Path;
                                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                                {
                                    context.Token = accessToken;
                                }
                                return Task.CompletedTask;
                            }
                        };
                    });

            return services;
        }
        public static async Task ConfigureMiddleWare(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                await IdentitySeed.SeedUserAsync(userManager, roleManager);
            }
        }

        public class CustomUserValidator<TUser> : IUserValidator<TUser> where TUser : class
        {
            public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
            {
                // This custom validator doesn't perform any validation, allowing duplicate usernames
                return Task.FromResult(IdentityResult.Success);
            }
        }


    }
}
