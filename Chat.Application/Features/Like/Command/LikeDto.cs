namespace Chat.Application.Features.Like.Command
{
    public class LikeDto
    {
        public string Id { get; set; } = default!;
        public string firstName { get; set; } = default!;
        public string lastName { get; set; } = default!;
        public int Age { get; set; } = default!;    
        public string? photoUrl { get; set; } = default!;
    }
}
