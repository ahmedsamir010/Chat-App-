namespace Chat.Application.Presistance.Contracts
{
    public interface IMessageRepository : IGenericRepository<Message>
    {
        Task<Pagination<MessageDto>> GetUserMessagesAsync(UserMessagesParams userMessages);

        Task<IEnumerable<MessageDto>> GetUserMessagesReadAsync(string currentUserName,string recipentuserName);

        Task<Group> GetMessageGroup(string groupName);
        void RemoveConnection(connection connection);

        void AddGroup(Group group);
        Task<Group> GetGroupForConnection(string connectionId);

    }
}
