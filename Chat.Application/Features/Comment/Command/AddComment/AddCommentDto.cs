namespace Chat.Application.Features.Comment.Command.AddComment
{
    public class AddCommentDto
    {
        public string ContentComment { get; set; } = null!;
        public IFormFile? Picture { get; set; } = null!;
        public int PostId { get; set; }
    }
}
