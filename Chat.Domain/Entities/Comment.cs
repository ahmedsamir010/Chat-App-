namespace Chat.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string ContentComment { get; set; } = null!;
        public string? PictureUrl { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? ModifiedDate { get; set; }
        public AppUser User { get; set; } = null!;
        public Post  Post { get; set; } = null!;
    }
}
