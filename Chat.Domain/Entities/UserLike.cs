namespace Chat.Domain.Entities
{
    public class UserLike
    {
        public AppUser SourceUser { get; set; } = default!;
        public string SourceUserId { get; set; } = default!;
        public AppUser LikedUser { get; set; } = default!;
        public string LikedUserId { get; set; } = default!;
    }
}
