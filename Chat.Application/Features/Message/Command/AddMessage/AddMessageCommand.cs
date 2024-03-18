using AutoMapper;
using Chat.Application.Features.Message.Validator;
using Chat.Application.Presistance.Contracts;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
namespace Chat.Application.Features.Message.Command.AddMessage
{
    public class AddMessageCommand(AddMessageDto messageDto) : IRequest<BaseCommonResponse>
    {
        public AddMessageDto AddMessageDto { get; } = messageDto;

        public class Handler(IConfiguration configuration, IMessageRepository messageRepository, UserManager<AppUser> userManager, IMapper mapper, IHttpContextAccessor httpContextAccessor) : IRequestHandler<AddMessageCommand, BaseCommonResponse>
        {
            private readonly IConfiguration _configuration = configuration;
            private readonly IMessageRepository _messageRepository = messageRepository;
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IMapper _mapper = mapper;
            private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

            public async Task<BaseCommonResponse> Handle(AddMessageCommand request, CancellationToken cancellationToken)
            {
                var validations = new MessageValidator();
                var response = new BaseCommonResponse();

                var recipientUser = await _userManager.FindByNameAsync(request.AddMessageDto.recipentUserName);

                if (recipientUser == null)
                {
                    return ResponseWithMessage(response, false, "Recipient username not found");
                }

                var messageValidations = await validations.ValidateAsync(request.AddMessageDto);

                if (!messageValidations.IsValid)
                {
                    return ResponseWithMessage(response, false, "Failed to add message", messageValidations.Errors.Select(x => x.ErrorMessage).ToList());
                }

                var result = _mapper.Map<Domain.Entities.Message>(request.AddMessageDto);
                var userId = _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    throw new ApplicationException("User ID not found in the claims.");
                }

                result.SenderId = userId!;
                result.Sender = await _userManager.FindByIdAsync(userId);
                result.SenderUserName = result.Sender.UserName;

                if (recipientUser != null)
                {
                    result.RecieptUserName = recipientUser.UserName!;
                    var recipient = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == result.RecieptUserName);

                    if (recipient is null)
                    {
                        return ResponseWithMessage(response, false, "Recipient username not found");
                    }

                    result.RecieptUserName = recipient.UserName!;
                    result.RecieptId = recipient.Id;

                    if (recipient.UserName == result.SenderUserName)
                    {
                        return ResponseWithMessage(response, false, "You can't send a message to yourself");
                    }
                }

                await _messageRepository.AddAsync(result);
                response.Data = new
                {
                    Id=result.Id,
                    senderUserName=result.SenderUserName,
                    content = result.Content,
                    senderId=result.SenderId,
                    recieptId=result.RecieptId,
                    recieptUserName=result.RecieptUserName,
                    dateRead=result.DateRead,
                    messageSend=result.MessageSend,
                    senderProfileUrl=result.Sender.Photos?.FirstOrDefault(x=>x.IsMain)?.Url,
                    recipientProfileUrl = _configuration["ApiUrl:Base"] + result.Recipient?.Photos.FirstOrDefault(x => x.IsMain)?.Url
                };
                return ResponseWithMessage(response, true, "Message created successfully");
            }

            private BaseCommonResponse ResponseWithMessage(BaseCommonResponse response, bool success, string message, List<string>? errors = null)
            {
                response.IsSuccess = success;
                response.Message = message;
                response.Errors = errors ?? new List<string>();
                return response;
            }
        }
    }
}
