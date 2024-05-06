namespace Chat.Application
{
    public class CurrentUserProvider
    {

        private readonly IHttpContextAccessor _httpContext;
        private readonly UserManager<AppUser> _userManager;
        public CurrentUserProvider(IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
        {
            _httpContext = httpContext;
            _userManager = userManager;
        }
        public async Task<AppUser?> GetCurrentUserAsync()
        {
            var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null)
            {
                var userId = userIdClaim.Value;
                var user = await _userManager.FindByIdAsync(userId);
                return user;
            }
            return null;
        }
    }
}
