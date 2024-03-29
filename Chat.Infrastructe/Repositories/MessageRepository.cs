using AutoMapper;
using AutoMapper.QueryableExtensions;
using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Helpers.Paginations;
using Chat.Application.Helpers.PaginationsMessages;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Chat.Infrastructe.Data;
using Microsoft.EntityFrameworkCore;
namespace Chat.Infrastructe.Repositories
{
    public class MessageRepository : GenericRepository<Message>, IMessageRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IMapper _mapper;


        public MessageRepository(ApplicationDbContext dbContext,IMapper mapper) : base(dbContext)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _dbContext.Groups.Include(x => x.Connections)
                .Where(x => x.Connections.Any(c => c.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
        public void RemoveConnection(connection connection)
        {
            _dbContext.Connections.Remove(connection);
        }
        public void AddGroup(Group group)
        {
            _dbContext.Groups.Add(group);
        }
        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _dbContext?.Groups?.Include(x => x.Connections)!.FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public async Task MarkMessagesAsRead(string currentUserName, string senderUserName)
        {
            var messagesToMarkAsRead = await _dbContext.Messages
                .Where(m => m.RecieptUserName == currentUserName && m.SenderUserName == senderUserName && m.DateRead == null)
                .ToListAsync();

            foreach (var message in messagesToMarkAsRead)
            {
                message.DateRead = DateTime.UtcNow;
            }

            await _dbContext.SaveChangesAsync();
        }
        public async Task<Pagination<MessageDto>> GetUserMessagesAsync(UserMessagesParams userMessages)
        {
            var query = _dbContext.Messages.OrderByDescending(x => x.MessageSend).AsQueryable();

            query = userMessages.container.ToLower() switch
            {
                "outbox" => query.Where(x => x.SenderUserName == userMessages.CurrentuserName),
                "inbox" => query.Where(x => x.RecieptUserName == userMessages.CurrentuserName),
                _ => query.Where(x => x.RecieptUserName == userMessages.CurrentuserName && x.DateRead == null),
            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return Pagination<MessageDto>.Create(messages,userMessages.PageNumber,userMessages.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetUserMessagesReadAsync(string currentUserName, string recipientUserName)
        {
            // Retrieves messages sent between two users.

            // Include related entities (Sender and Recipient) to avoid lazy loading.
            var messages = _dbContext.Messages
                .Include(x => x.Sender).ThenInclude(x => x.Photos)
                .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                .Where(x =>
                    (x.Recipient.UserName == currentUserName && x.Sender.UserName == recipientUserName) || // Messages from current user to recipient
                    (x.Recipient.UserName == recipientUserName && x.Sender.UserName == currentUserName)) // Messages from recipient to current user
                .OrderByDescending(x => x.MessageSend)
                .ToList();

            // Identify unread messages sent to the current user.
            var unreadMessages = messages.Where(x => x.DateRead == null && x.Recipient.UserName == currentUserName).ToList();

            // Mark unread messages as read and save changes to the database.
            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.UtcNow;
                    //_dbContext.Messages.Update(message); // No need to update explicitly, context tracks changes.
                }
                await _dbContext.SaveChangesAsync();
            }

            // Map messages to DTOs and return the result.
            return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }
}
