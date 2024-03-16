namespace Chat.Application.Features.Message.Query.GetAllMessages
{
    public class MessageReturnDto
    {
        public string RecieptUserName { get; set; } = default!;
        public string SenderUserName { get; set; } = default!;
        public DateTime? DateRead { get; set; } 
        public string ContentMessage { get; set; } = default!;
    }
}
