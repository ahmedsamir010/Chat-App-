using System.ComponentModel.DataAnnotations;
namespace Chat.Application.Features.Post.Command.AddPost
{
    public class AddPostDto
    {
        [Required(ErrorMessage = "Must Add Content")]
        public string ContentPost { get; set; } = null!;
        public IFormFile? Picture { get; set; }
    }
}
