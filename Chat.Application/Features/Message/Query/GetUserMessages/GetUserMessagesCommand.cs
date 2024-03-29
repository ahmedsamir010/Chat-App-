using Chat.Application.Helpers.Paginations;
using Chat.Application.Helpers.PaginationsMessages;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Chat.Application.Features.Message.Query.GetUserMessages
{
    public class GetUserMessagesCommand(UserMessagesParams messagesParams) : IRequest<Pagination<MessageDto>>
    {
        private readonly UserMessagesParams _messagesParams = messagesParams;

        class Handler(IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor, UserManager<AppUser> userManager) : IRequestHandler<GetUserMessagesCommand, Pagination<MessageDto>>
        {
            private readonly IMessageRepository _messageRepository = messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<Pagination<MessageDto>> Handle(GetUserMessagesCommand request, CancellationToken cancellationToken)
            {
                var userId=_httpContextAccessor?.HttpContext?.User.Claims.FirstOrDefault(x=>x.Type == ClaimTypes.NameIdentifier)?.Value;
                if(userId is not null)
                {
                    var user=await _userManager.FindByIdAsync(userId);
                    if(user is not null)
                    {
                        request._messagesParams.CurrentuserName = user.UserName!;
                    }
                }
                var messages = await _messageRepository.GetUserMessagesAsync(request._messagesParams);
              
                return messages;
            }
        }
    }
}
