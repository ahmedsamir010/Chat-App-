using Chat.Application.Features.Comment.Command.AddComment;
using Chat.Application.Features.Comment.Command.DeleteComment;
using Chat.Application.Features.Comment.Command.UpdateComment;
using Chat.Application.Features.Comment.Query.GetComments;
namespace Chat.API.Controllers
{

    public class CommentController(IMediator mediator) : BaseController(mediator)
    {
        private readonly IMediator _mediator = mediator;

        [HttpPost("Add-Comment")]
        public async Task<IActionResult> AddComment(AddCommentDto addCommentDto)
        {
            var command = new AddCommentCommand(addCommentDto);
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok(result);
            }
            return BadRequest("Failed to add comment.");
        }

        [HttpGet("Get-All-Comments-For-Post")]
        public async Task<ActionResult<IEnumerable<CommentsDto>>> GetAllCommentsForPost(int PostId)
        {
            var query = await _mediator.Send(new GetCommentsForPostQuery(PostId));
            if (query is null)
            {

                return NotFound(new ApiResponse(404));
            }
            return Ok(query);
        }

        [HttpDelete("Delete-Comment")]
        public async Task<IActionResult> DeleteComment(int Id)
        {
            var command = new DeleteCommentCommand(Id);
            var response = await _mediator.Send(command);
            if (response.ResponseStatus == ResponseStatus.Success)
            {
                return Ok(new ApiResponse(200, "Comment deleted successfully."));
            }
            else if (response.ResponseStatus == ResponseStatus.NotFound)
            {
                return NotFound(new ApiResponse(404, "Comment not found."));
            }
            else if (response.ResponseStatus == ResponseStatus.Unauthorized)
            {
                return Unauthorized(new ApiResponse(401, "Unauthorized: You are not authorized to delete this Comment."));
            }
            return StatusCode(500, new ApiResponse(500, "Internal server error."));
        }

        [HttpPut("Update-Comment")]
        public async Task<IActionResult> UpdateComment(UpdateCommentDto updateCommentDto)
        {
            var command = new UpdateCommentCommand(updateCommentDto);
            var response = await _mediator.Send(command);

            if (response.ResponseStatus == ResponseStatus.Success)
            {
                return Ok(new ApiResponse(200, "Comment updated successfully."));
            }
            else if (response.ResponseStatus == ResponseStatus.NotFound)
            {
                return NotFound(new ApiResponse(404, "Comment not found."));
            }
            else if (response.ResponseStatus == ResponseStatus.Unauthorized)
            {
                return Unauthorized(new ApiResponse(401, "Unauthorized: You are not authorized to update this Comment."));
            }
            return StatusCode(500, new ApiResponse(500));
        }










    }
}
