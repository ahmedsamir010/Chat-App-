namespace Chat.Application.Features.Comment.Query.GetComments
{
    public class CommentsDto
    {
        public int Id { get; set; }
        public string ContentComment { get; set; } = null!;
        public int PostId { get; set; }
        public string? PictureUrl { get; set; }
        public string UserName { get; set; } = null!;
    }
}
