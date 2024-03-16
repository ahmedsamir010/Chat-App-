using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
namespace Chat.Application.Features.Message.Query.GetMessageUserRead
{
    public class GetMessageUserReadQuery : IRequest<IEnumerable<MessageDto>>
    {
        public string RecipentUserName { get; set; }
        public string currentUserName { get; set; }
        public GetMessageUserReadQuery(string recipentUserName)
        {
            RecipentUserName = recipentUserName;
        }
        class Handler : IRequestHandler<GetMessageUserReadQuery, IEnumerable<MessageDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMessageRepository _messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor;

            public Handler(UserManager<AppUser> userManager,IMessageRepository messageRepository,IHttpContextAccessor httpContextAccessor)
            {
                _userManager = userManager;
                _messageRepository = messageRepository;
                _httpContextAccessor = httpContextAccessor;
            }
            public async Task<IEnumerable<MessageDto>> Handle(GetMessageUserReadQuery request, CancellationToken cancellationToken)
            {
                var userId=_httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)?.Value;
                var userName=await _userManager.FindByIdAsync(userId!);
                var query = await _messageRepository.GetUserMessagesReadAsync(userName!.UserName!, request.RecipentUserName);
                if (query is not null) return query;
                return null;
            }
        }


    }
}
