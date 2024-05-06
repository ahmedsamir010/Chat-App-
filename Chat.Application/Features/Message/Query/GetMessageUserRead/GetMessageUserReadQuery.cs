namespace Chat.Application.Features.Message.Query.GetMessageUserRead
{
    public class GetMessageUserReadQuery(string recipentUserName) : IRequest<IEnumerable<MessageDto>>
    {
        public string RecipentUserName { get; set; } = recipentUserName;
      //  public string currentUserName { get; set; }
        class Handler(UserManager<AppUser> userManager, IMessageRepository messageRepository, IHttpContextAccessor httpContextAccessor) : IRequestHandler<GetMessageUserReadQuery, IEnumerable<MessageDto>>
        {
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IMessageRepository _messageRepository = messageRepository;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            public async Task<IEnumerable<MessageDto>> Handle(GetMessageUserReadQuery request, CancellationToken cancellationToken)
            {
                var userId=_httpContextAccessor?.HttpContext?.User?.Claims?.FirstOrDefault(x=>x.Type==ClaimTypes.NameIdentifier)?.Value;
                var userName=await _userManager.FindByIdAsync(userId!);
                if(userName is not null)
                {
                var query = await _messageRepository.GetUserMessagesReadAsync(userName!.UserName!, request.RecipentUserName);
                if (query is not null) return query;
                }
                return null;
            }
        }


    }
}
