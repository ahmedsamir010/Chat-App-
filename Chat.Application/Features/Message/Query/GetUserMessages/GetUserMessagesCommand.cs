using Chat.Application.Helpers.PaginationsMessages;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Chat.Infrastructe.Helpers;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Chat.Application.Features.Message.Query.GetUserMessages
{
    public class GetUserMessagesCommand:IRequest<Pagination<MessageDto>>
    {
        private readonly UserMessagesParams _messagesParams;

        public GetUserMessagesCommand(UserMessagesParams messagesParams)
        {
            _messagesParams = messagesParams;
        }


        class Handler : IRequestHandler<GetUserMessagesCommand, Pagination<MessageDto>>
        {
            private readonly IMessageRepository _messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IMessageRepository messageRepository,IHttpContextAccessor httpContextAccessor,UserManager<AppUser> userManager)
            {
                _messageRepository = messageRepository;
                _httpContextAccessor = httpContextAccessor;
                _userManager = userManager;
            }
            public async Task<Pagination<MessageDto>> Handle(GetUserMessagesCommand request, CancellationToken cancellationToken)
            {
                var userId=_httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier)?.Value;
                if(userId is not null)
                {
                    var user=await _userManager.FindByIdAsync(userId);
                    if(user is not null)
                    {
                        request._messagesParams.userName = user.UserName!;
                    }
                }
                var messages = await _messageRepository.GetUserMessagesAsync(request._messagesParams);
              
                return messages;
            }
        }
    }
}
