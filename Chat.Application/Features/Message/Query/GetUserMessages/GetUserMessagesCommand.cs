using Chat.Application.Helpers.PaginationsMessages;
namespace Chat.Application.Features.Message.Query.GetUserMessages
{
    public class GetUserMessagesCommand(UserMessagesParams messagesParams) : IRequest<Pagination<MessageDto>>
    {
        private readonly UserMessagesParams _messagesParams = messagesParams;

        class Handler(IMessageRepository messageRepository) : IRequestHandler<GetUserMessagesCommand, Pagination<MessageDto>>
        {
            private readonly IMessageRepository _messageRepository = messageRepository;

            public async Task<Pagination<MessageDto>> Handle(GetUserMessagesCommand request, CancellationToken cancellationToken)
            {
                var messages = await _messageRepository.GetUserMessagesAsync(request._messagesParams);
                return messages;
            }
        }
    }
}
