namespace Chat.Application.Helpers.PaginationsMessages
{
    public class UserMessagesParams : PaginationParams
    {
        public string? container { get; set; } = "unRead";
        public string? otherSecondUser { get; set; }
    }
}
