using AutoMapper;
using Chat.Application.Helpers.PaginationsMessages;
using Chat.Application.Presistance.Contracts;
using MediatR;

namespace Chat.Application.Features.Message.Query.GetAllMessages
{
    public class GetAllMessageQuery:IRequest<List<MessageReturnDto>>
    {
        class Handler(IMessageRepository messageRepository, IMapper mapper) : IRequestHandler<GetAllMessageQuery, List<MessageReturnDto>>
        {
            private readonly IMessageRepository _messageRepository = messageRepository;
            private readonly IMapper _mapper = mapper;

            public async Task<List<MessageReturnDto>> Handle(GetAllMessageQuery request, CancellationToken cancellationToken)
            {
                try
                {
                    var allMessages = await _messageRepository.GetAllAsync();
                    var mappedMessages=_mapper.Map<List<MessageReturnDto>>(allMessages);
                    return mappedMessages;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }
    }
}
