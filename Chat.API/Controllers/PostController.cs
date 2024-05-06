using Chat.Application.Features.Accounts.Command.UpdateCurrentUser;
using Chat.Application.Features.Post.Command.AddPost;
using Chat.Application.Features.Post.Command.DeletePost;
using Chat.Application.Features.Post.Command.UpdatePost;
using Chat.Application.Features.Post.Query.GetAllPost;

namespace Chat.API.Controllers
{
    public class PostController(IMediator mediator) : BaseController(mediator)
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("Add-Post")]
        public async Task<IActionResult> AddPost(AddPostDto addPostDto)
        {
            var command = new AddPostCommand(addPostDto);
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest("Failed to add post.");
        }

        [HttpGet("Get-All-Posts")]
        public async Task<ActionResult<IEnumerable<AddPostDto>>> GetAllPosts()
        {
            var query = await _mediator.Send(new GetAllPostsQuery());
            if (query is not null)
            {
                return Ok(query);
            }
            return Ok(new List<AddPostDto>());
        }

        [HttpDelete("Delete-Post")]
        public async Task<IActionResult> DeletePost(int id)
        {
            var command = new DeletePostCommand(id);
            var response = await _mediator.Send(command);

            if (response.ResponseStatus == ResponseStatus.Success)
            {
                return Ok(new ApiResponse(200, "Post deleted successfully."));
            }
            else if (response.ResponseStatus == ResponseStatus.NotFound)
            {
                return NotFound(new ApiResponse(404, "Post not found."));
            }
            else if (response.ResponseStatus == ResponseStatus.Unauthorized)
            {
                return Unauthorized(new ApiResponse(401, "Unauthorized: You are not authorized to delete this post."));
            }

            return StatusCode(500, new ApiResponse(500, "Internal server error."));

        }


        [HttpPut("Update-Post")]
        public async Task<IActionResult> UpdatePost(UpdatePostDto updatePostDto)
        {
            var command = new UpdatePostCommand(updatePostDto);
            var response = await _mediator.Send(command);

            if (response.ResponseStatus == ResponseStatus.Success)
            {
                return Ok(new ApiResponse(200, "Post updated successfully."));
            }
            else if (response.ResponseStatus == ResponseStatus.NotFound)
            {
                return NotFound(new ApiResponse(404, "Post not found."));
            }
            else if (response.ResponseStatus == ResponseStatus.Unauthorized)
            {
                return Unauthorized(new ApiResponse(401, "Unauthorized: You are not authorized to update this post."));
            }
            return StatusCode(500, new ApiResponse(500));

        }






    }
}
