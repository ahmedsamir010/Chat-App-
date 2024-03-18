using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace Chat.Application.Features.Accounts.Query.CheckUserNameOrEmail
{
    public class CheckUserNameOrEmailQuery : IRequest<bool>
    {
        public string SearchTerm { get; set; } = default!;
        public CheckUserNameOrEmailQuery(string searchTerm)
        {
            SearchTerm = searchTerm;
        }
        class Handler : IRequestHandler<CheckUserNameOrEmailQuery, bool>
        {
            private readonly UserManager<AppUser> _userManager;
            public Handler(UserManager<AppUser> userManager)
                    => _userManager = userManager;
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
