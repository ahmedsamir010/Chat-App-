using Chat.Domain.Common;

namespace Chat.Domain.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string ContentPost { get; set; } = null!;
        public string? PictureUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public AppUser User { get; set; } = null!;
    }
}
