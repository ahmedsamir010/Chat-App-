namespace Chat.Application.Features.Accounts.Query.CheckUserNameOrEmail
{
    public class CheckUserNameOrEmailQuery(string searchTerm) : IRequest<bool>
    {
        public string SearchTerm { get; set; } = searchTerm;

        class Handler(UserManager<AppUser> userManager) : IRequestHandler<CheckUserNameOrEmailQuery, bool>
        {
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<bool> Handle(CheckUserNameOrEmailQuery request, CancellationToken cancellationToken)
            {
                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var result = request.SearchTerm.Contains('@');
                    if (result is true)
                    {
                        var email = await _userManager.FindByEmailAsync(request.SearchTerm);
                        if (email is not null) return true;
                    }
                    else
                    {
                        var userName = await _userManager.FindByNameAsync(request.SearchTerm);
                        if (userName is not null) return true;
                    }
                }
                return false;
            }
        }
    }
}
