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
                "outbox" => query.Where(x => x.SenderUserName == userMessages.userName),
                "inbox" => query.Where(x => x.RecieptUserName == userMessages.userName),
                _ => query.Where(x => x.RecieptUserName == userMessages.userName && x.DateRead == null),
            };
            var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);
            return Pagination<MessageDto>.Create(messages,userMessages.PageNumber,userMessages.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetUserMessagesReadAsync(string currentUserName, string recipentuserName)
        {
            var messages = _dbContext.Messages
                                   .Include(x => x.Sender).ThenInclude(x => x.Photos)
                                   .Include(x => x.Recipient).ThenInclude(x => x.Photos)
                                   .Where(x=>x.Recipient.UserName == currentUserName
                                          && x.Sender.UserName == recipentuserName || x.Recipient.UserName == recipentuserName
                                          && x.Sender.UserName == currentUserName).OrderByDescending(x=>x.MessageSend).ToList();

            var unReadMessage = messages.Where(x => x.DateRead == null && x.Recipient.UserName == currentUserName).ToList();

            if (unReadMessage.Any())
            {
                foreach (var item in unReadMessage)
                {
                    item.DateRead = DateTime.UtcNow;
                    //_dbContext.Messages.Update(item);
                }
                   await _dbContext.SaveChangesAsync();
            }
           return _mapper.Map<IEnumerable<MessageDto>>(messages);
        }
    }
}
