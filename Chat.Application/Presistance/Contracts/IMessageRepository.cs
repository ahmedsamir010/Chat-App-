using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Helpers.PaginationsMessages;
using Chat.Domain.Entities;
using Chat.Infrastructe.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Presistance.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Pagination<MessageDto>> GetUserMessagesAsync(UserMessagesParams userMessages);

        Task<IEnumerable<MessageDto>> GetUserMessagesReadAsync(string currentUserName,string recipentuserName);
        Task MarkMessagesAsRead(string currentUserName, string senderUserName);

        Task<Group> GetMessageGroup(string groupName);
        void RemoveConnection(connection connection);

        void AddGroup(Group group);
        Task<Group> GetGroupForConnection(string connectionId);

    }
}
