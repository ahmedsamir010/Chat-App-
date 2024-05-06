namespace Chat.Application.Features.Accounts.Query.GetCurrentUser
{
    public class GetCurrentUserQuery : IRequest<UserDto>
    {
        class Handler : IRequestHandler<GetCurrentUserQuery, UserDto>
        {
            private readonly IHttpContextAccessor _httpContext;
            private readonly UserManager<AppUser> _userManager;
            private readonly ITokenService _tokenService;

            public Handler(IHttpContextAccessor httpContext, UserManager<AppUser> userManager, ITokenService tokenService)
                => (_httpContext, _userManager, _tokenService) = (httpContext, userManager, tokenService);

            public async Task<UserDto> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
            {
                var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                {
                    var userId = userIdClaim.Value;
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        return new UserDto
                        {
                            UserId = user.Id,
                            Email = user.Email!,
                            FirstName = user.FirstName,
                            LastName = user.LastName,
                            Token=await _tokenService.CreateTokenAsync(user)
                        };
                    }
                    else
                    {
                        Console.WriteLine($"User not found by ID: {userId}");
                    }
                }
                else
                {
                    // Log or handle the case where the NameIdentifier claim is not present
                    Console.WriteLine("NameIdentifier claim not found in user's claims");
                }

                return null; // or throw an exception, depending on your requirements
            }
        }
    }
}
