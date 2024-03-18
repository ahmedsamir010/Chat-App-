using Chat.Application.Features.Message.Command.AddMessage;
using Chat.Application.Features.Message.Query.GetMessageUserRead;
using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Helpers.PaginationsMessages;
namespace Chat.API.Controllers
{
    public class MessageController(IMediator mediator) : BaseController(mediator)
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Adds a new message.
        /// </summary>
        /// <param name="addMessageDto">The message to add.</param>
        /// <returns>Returns the result of the operation.</returns>
        [HttpPost("AddMessage")]
        public async Task<ActionResult> AddMessage([FromBody] AddMessageDto addMessageDto)
        {
            var command = new AddMessageCommand(addMessageDto);
            var response = await _mediator.Send(command);
            return response.IsSuccess ? Ok(response) : BadRequest(new { response.Message, response.Errors });
        }
        /// <summary>
        /// Retrieves all messages for the current user.
        /// </summary>
        /// <param name="messagesParams">Parameters for filtering, sorting, and pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a paginated list of messages belonging to the current user.</returns>

        [HttpGet("GetUserMessages")]

        public async Task<ActionResult<MessageDto>> GetMessages([FromQuery] UserMessagesParams messagesParams, CancellationToken cancellationToken)
        {
            var messages = await _mediator.Send(new GetUserMessagesCommand(messagesParams), cancellationToken);
            Response.AddPaginationHeaders(messages.CurrentPage, messages.TotalPages, messages.Count, messages.PageSize);
            if (messages is not null)
            {
                return Ok(messages);
            }
            return NotFound("No Message");
        }

        /// <summary>
        /// Retrieves messages read by the specified user.
        /// </summary>
        /// <param name="userName">The username of the recipient whose read messages are to be retrieved.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Returns a collection of messages read by the specified user.</returns>
        [HttpGet("Get-message-read/{userName}")]
        public async Task<ActionResult<MessageDto>> GetMessageRead(string userName, CancellationToken ct)
        {
            var query = await _mediator.Send(new GetMessageUserReadQuery(userName), ct);
            if (query is not null)
            {
                return Ok(query);
            }
            return NotFound("No Found Read Messages");
        }
    }
}
