using AutoMapper;
using Chat.Application.Features.Message.Command.AddMessage;
using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Chat.Infrastructe.Data;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace Chat.API.signalR
{
    [Authorize]
    public class MessageHub(IHubContext<PresenceHub> precencehub, ApplicationDbContext dbContext, IMessageRepository messageRepository, UserManager<AppUser> userManager, IMapper mapper) : Hub
    {
        private readonly IHubContext<PresenceHub> _precencehub = precencehub;
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly IMessageRepository _messageRepository = messageRepository;
        private readonly IMapper _mapper = mapper;
        private readonly UserManager<AppUser> _userManager = userManager;

        public override async Task OnConnectedAsync()
        {
            var userId = Context?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var currentUserName = await _userManager.FindByIdAsync(userId);
            var httpContext = Context?.GetHttpContext();

            var otherSecondUser = httpContext?.Request.Query["user"].ToString();
            var groupName = GetGroupName(currentUserName?.UserName!, otherSecondUser!);

            await Groups.AddToGroupAsync(Context!.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await Clients.Group(groupName).SendAsync("UpdateGroup", group);

            // Retrieve messages from the repository
            var messages = await _messageRepository.GetUserMessagesReadAsync(currentUserName!.UserName!, otherSecondUser!);

            // Transform the data to an array
            var messagesArray = messages?.ToList() ?? new List<MessageDto>();

            await Clients.Group(groupName).SendAsync("RecieveMessageRead", messagesArray);
        }

        private async Task<Group> AddToGroup(string? groupName)
        {
            var group = await _messageRepository.GetMessageGroup(groupName);
            var userId = Context?.User?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var currentUserName = await _userManager.FindByIdAsync(userId);
            var connection = new connection(Context.ConnectionId, currentUserName.UserName);
            if (group == null)
            {
                group = new Group(groupName);
             _messageRepository.AddGroup(group);
            }

            group.Connections.Add(connection);
           var result= await _dbContext.SaveChangesAsync();
            return group;
        }
        public async Task SendMessage(AddMessageDto addMessageDto)
        {
            var userId = Context.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var senderUserName = await _userManager.FindByIdAsync(userId);
            var sender = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == senderUserName.UserName);

            var recipient = await _userManager.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == addMessageDto.recipentUserName);
            var mappedMessage = _mapper.Map<Message>(addMessageDto);
            mappedMessage.SenderUserName = senderUserName?.UserName;

            if (recipient != null)
            {
                mappedMessage.RecieptId = recipient.Id;
                mappedMessage.RecieptUserName = addMessageDto.recipentUserName;
            }

            await _messageRepository.AddAsync(mappedMessage);

            var groupName = GetGroupName(sender.UserName, recipient.UserName);

            var group = await _messageRepository.GetMessageGroup(groupName);

            // Check if the recipient is connected and the Messages tab is open
            var isRecipientConnected = group.Connections.Any(x => x.Username == recipient.UserName);
            var isMessagesTabOpen = isRecipientConnected; // Modify this based on your actual logic

            if (isMessagesTabOpen)
            {
                mappedMessage.DateRead = DateTime.UtcNow;
            }
            else
            {
                var connections = await PresenceTracker.GetConnectionForUsers(recipient.UserName);
                if (connections != null)
                {
                    await _precencehub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { username = sender.UserName, knownAs = sender.KnownAs });
                }
            }

            mappedMessage.SenderId = sender.Id;
            await _messageRepository.AddAsync(mappedMessage);
            await _dbContext.SaveChangesAsync();

            await Clients.Group(groupName).SendAsync("NewMessage", mappedMessage);
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _messageRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _messageRepository.RemoveConnection(connection);
            await _dbContext.SaveChangesAsync();
            return group;
            throw new HubException("Failed to removed from group");
        }
        private string GetGroupName(string callerFirst, string otherSecond)
        {
            var stringCompare = string.CompareOrdinal(callerFirst, otherSecond) < 0;
            return stringCompare ? $"{callerFirst}-{otherSecond}" : $"{otherSecond}-{callerFirst}";
        }


        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var group = await RemoveFromMessageGroup();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup");
            await base.OnDisconnectedAsync(exception);
        }
    }
}
