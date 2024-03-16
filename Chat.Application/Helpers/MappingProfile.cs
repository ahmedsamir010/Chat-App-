using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Features.Message.Query.GetAllMessages;
using Chat.Domain.Entities;
using Chat.Application.ExtensionMethods;
using Chat.Application.Features.Accounts.Command.UpdateCurrentUser;
using Chat.Application.Helpers;
using Chat.Application.Features.Accounts.Command.Register;
using Chat.Application.Features.Message.Command.AddMessage;
using Chat.Application.Features.Message.Query.GetUserMessages;

namespace Chat.Application.MappingProfiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Message, AddMessageDto>()
                .ForMember(dest => dest.contentMessage, src => src.MapFrom(src => src.Content)).ReverseMap();
            CreateMap<Message, MessageReturnDto>().ReverseMap();

            CreateMap<Photo, PhotoDto>()
                .ForMember(dest => dest.Url, opt => opt.MapFrom<UserPhotoResolver>())
                .ReverseMap();

            CreateMap<AppUser, MemberDto>()
                      .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom<PhotoMemberResolver>())
                      .ForMember(dest => dest.Age, opt => opt.MapFrom(src => src.DateOfBirth.CalculateAge()))
                     .ReverseMap();

            CreateMap<UpdateCurrentUserDto, AppUser>().ReverseMap();

            CreateMap<AppUser, RegisterDto>().ReverseMap();

            CreateMap<Message, MessageDto>()
        .ForMember(d => d.RecipientProfileUrl, x => x.MapFrom(src => MapProfileUrl(src.Recipient)))
        .ForMember(d => d.SenderProfileUrl, x => x.MapFrom(src => MapProfileUrl(src.Sender)))
        .ReverseMap();

            CreateMap<DateTime, DateTime>().ConvertUsing(x => DateTime.SpecifyKind(x, DateTimeKind.Utc));

        }

        private static string MapProfileUrl(AppUser user)
        {
            if (user != null && user.Photos != null && user.Photos.Any(x => x.IsMain))
            {
                var mainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);
                return "https://localhost:7171/" + mainPhoto.Url;
            }
            return "";
        }
    }
}
