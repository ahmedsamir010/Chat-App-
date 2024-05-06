namespace Chat.Application.Features.Accounts.Command.Register
{
    public class RegisterDto
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string Gender { get; set; } = default!;
        public DateTime DateOfBirth { get; set; } = default!;
    }
}
