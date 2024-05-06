using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Features.Message.Query.GetAllMessages;
using Chat.Domain.Entities;
using Chat.Application.ExtensionMethods;
using Chat.Application.Features.Accounts.Command.UpdateCurrentUser;
using Chat.Application.Features.Accounts.Command.Register;
using Chat.Application.Features.Message.Command.AddMessage;
using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Features.Accounts.Command.DeleteProfile;
using Chat.Application.Features.Accounts.Query.GetCurrentUser;
using Chat.Application.Features.Accounts.Query.GetAllBlockedUser;
using Chat.Application.Helpers.Resolver;
using Chat.Application.Features.Post.Command.AddPost;
using Chat.Application.Features.Post.Query.GetAllPost;
using Chat.Application.Features.Post.Command.UpdatePost;

namespace Chat.Application.Helpers.AutoMapper
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
            CreateMap<BlockProfileDto, AppUser>().ReverseMap();


            CreateMap<UserDto, AppUser>().ReverseMap();



            CreateMap<AppUser, RegisterDto>().ReverseMap();

            CreateMap<AppUser, BlockedUsersDto>()
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src => CalculateAge(src.DateOfBirth)))
                .ReverseMap();


            CreateMap<Post, AddPostDto>().ReverseMap();
            CreateMap<Post, ReturnPostDto>().ReverseMap();

            CreateMap<Post, UpdatePostDto>().ReverseMap();







            CreateMap<Message, MessageDto>()
        .ForMember(d => d.RecipientProfileUrl, x => x.MapFrom(src => MapProfileUrl(src.Recipient)))
        .ForMember(d => d.SenderProfileUrl, x => x.MapFrom(src => MapProfileUrl(src.Sender)))
        .ReverseMap();

            CreateMap<DateTime, DateTime>().ConvertUsing(x => DateTime.SpecifyKind(x, DateTimeKind.Utc));

        }
        private static int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--; // Adjust age if birthday hasn't occurred yet this year
            return age;
        }
        private static string MapProfileUrl(AppUser user)
        {
            if (user != null && user.Photos != null && user.Photos.Any(x => x.IsMain))
            {
                var mainPhoto = user.Photos.FirstOrDefault(x => x.IsMain);
                return "https://localhost:7171/" + mainPhoto!.Url;
            }
            return "";
        }
    }
}
