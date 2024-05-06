namespace Chat.Application.Features.Accounts.Command.DeleteProfile
{
    public class BlockProfileDto
    {
        public string UserId { get; set; } = null!;
        public int BlockDayes { get; set; }
    }
}
