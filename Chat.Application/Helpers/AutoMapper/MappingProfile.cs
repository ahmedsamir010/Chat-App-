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

            CreateMap<Comment, AddCommentDto>().ReverseMap();

            CreateMap<Comment, CommentsDto>()
                     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                     .ForMember(dest => dest.ContentComment, opt => opt.MapFrom(src => src.ContentComment))
                     .ForMember(dest => dest.PostId, opt => opt.MapFrom(src => src.Post.Id))
                     .ForMember(dest => dest.PictureUrl, opt => opt.MapFrom(src => src.PictureUrl))
                     .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<Comment, UpdateCommentDto>().ReverseMap();



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
            if (dateOfBirth.Date > today.AddYears(-age)) age--; 
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
