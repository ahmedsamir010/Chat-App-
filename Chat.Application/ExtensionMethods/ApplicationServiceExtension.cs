﻿using Chat.Application.Helpers.AutoMapper;
using Microsoft.Extensions.DependencyInjection;
namespace Chat.Application.ExtensionMethods
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServiceExtension(this IServiceCollection service)
        {
            service.AddAutoMapper(typeof(MappingProfile));
            service.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());
            service.AddScoped<logUserActivity>();
            return service;
        }
    }
}
