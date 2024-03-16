using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Domain.Entities;
using Microsoft.Extensions.Configuration;

namespace Chat.Application.Helpers
{
    public class UserPhotoResolver : IValueResolver<Photo, PhotoDto, string>
    {
        private readonly IConfiguration _configuration;

        public UserPhotoResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(Photo source, PhotoDto destination, string destMember, ResolutionContext context)
        {
            string? apiUrl = _configuration.GetSection("ApiUrl")["Base"];
            var photoUrl = source.IsMain;
            return string.IsNullOrEmpty(source?.Url) ? "" : apiUrl + source.Url;
        }
    }
}
