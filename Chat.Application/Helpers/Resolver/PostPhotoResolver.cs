namespace Chat.Application.Helpers.Resolver
{
    public class PostPhotoResolver : IValueResolver<Post, AddPostDto, string>
    {
        private readonly string _apiBaseUrl;

        public PostPhotoResolver(IConfiguration configuration)
        {
            _apiBaseUrl = configuration.GetSection("ApiUrl")["Base"] ?? string.Empty;
        }

        public string Resolve(Post source, AddPostDto destination, string destMember, ResolutionContext context)
        {
            // Assuming PictureUrl is the URL where the picture is stored
            return string.IsNullOrEmpty(source?.PictureUrl) ? "" : _apiBaseUrl + source.PictureUrl;
        }
    }
}
