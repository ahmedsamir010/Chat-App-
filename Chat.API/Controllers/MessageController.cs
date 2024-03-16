using Chat.Application.Features.Message.Command.AddMessage;
using Chat.Application.Features.Message.Query.GetAllMessages;
using Chat.Application.Features.Message.Query.GetMessageUserRead;
using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Helpers.Paginations;
using Chat.Application.Helpers.PaginationsMessages;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    public class MessageController : BaseController
    {
        private readonly IMediator _mediator;

        public MessageController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("AddMessage")]
        public async Task<ActionResult> AddMessage([FromBody] AddMessageDto addMessageDto)
        {
            var command = new AddMessageCommand(addMessageDto);
            var response = await _mediator.Send(command);
            return response.IsSuccess ? Ok(response) : BadRequest(new { response.Message, response.Errors });
        }
       
        /// <summary>
        /// Get All messages only For Current User
        /// </summary>
        /// <param name="messagesParams"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>

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


        [HttpGet("Get-message-read/{userName}")]
        public async Task<ActionResult<MessageDto>> GetMessageRead(string userName,CancellationToken ct)
        {
            var query = await _mediator.Send(new GetMessageUserReadQuery(userName), ct);
            if(query is not null)
            {
                return Ok(query);
            }
            return NotFound("No Found Read Messages");
        }

        //[HttpPost("DeleteMessage/{id}")]
        //public async Task<ActionResult> DeleteMessage(int id)
        //{
            
        //}


    }
}
