using Chat.Application.Features.Accounts.Query.GetAllUsers;

namespace Chat.Application.Features.Accounts.Query.GetAllBlockedUser
{
    public class BlockedUsersDto
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public int Age { get; set; }
        public bool? IsBlocked { get; set; }


    }
}
