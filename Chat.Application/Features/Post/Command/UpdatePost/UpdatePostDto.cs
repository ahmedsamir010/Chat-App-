using Microsoft.AspNetCore.Http;

namespace Chat.Application.Features.Post.Command.UpdatePost
{
    public class UpdatePostDto
    {
        public int Id { get; set; }
        public string ContentPost { get; set; } = null!;
        public IFormFile? Picture { get; set; }
    }
}
