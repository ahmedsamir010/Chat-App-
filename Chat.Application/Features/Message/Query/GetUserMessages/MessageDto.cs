using Chat.Domain.Entities;

namespace Chat.Application.Features.Message.Query.GetUserMessages
{
    public class MessageDto 
    {
        public int Id { get; set; }
        // Sender
        public string? SenderId { get; set; }
        public string? SenderUserName { get; set; }
        public string? SenderProfileUrl { get; set; }
        // Recipent
        public string? RecieptId { get; set; }
        public string? RecieptUserName { get; set; } 
        public string? RecipientProfileUrl { get; set; } 
        public string? Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSend { get; set; }
    }
}
