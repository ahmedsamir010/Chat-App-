namespace Chat.Application.Features.Post.Query.GetAllPost
{
    public class ReturnPostDto
    {
        public int Id { get; set; }
        public string ContentPost { get; set; } = null!;
        public string? PictureUrl { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string User { get; set; } = null!;
    }
}
