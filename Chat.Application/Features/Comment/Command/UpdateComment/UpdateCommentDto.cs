namespace Chat.Application.Features.Comment.Command.UpdateComment
{
    public class UpdateCommentDto
    {
        public int Id { get; set; }
        public string ContentComment { get; set; } = null!;
        public IFormFile? Picture { get; set; }
    }
}
