namespace Chat.Application.Helpers.PaginationsMessages
{
    public class UserMessagesParams : PaginationParams
    {
        public string? userName { get; set; } = default!;
        public string? container { get; set; } = "unRead";
        public string? otherSecondUser { get; set; }
    }
}
