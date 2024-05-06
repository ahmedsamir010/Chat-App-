namespace Chat.Application.Features.Accounts.Command.VerifyEmai
{
    public class VerificationDto
    {
        public string  Email { get; set; } = string.Empty;
        public string VerificationCode { get; set; } = string.Empty;
    }
}
