namespace Chat.Application.Helpers.Resolver
{
    public class PhotoMemberResolver : IValueResolver<AppUser, MemberDto, string>
    {
        private readonly IConfiguration _configuration;

        public PhotoMemberResolver(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Resolve(AppUser source, MemberDto destination, string destMember, ResolutionContext context)
        {
            string? apiUrl = _configuration.GetSection("ApiUrl")["Base"];
            var photo = source?.Photos?.FirstOrDefault(u => u.IsMain)?.Url;
            return photo is not null ? apiUrl + photo : "";
        }
    }
}
