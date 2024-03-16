using Chat.Domain.Entities;
using Chat.Infrastructe.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
[Authorize]
public class PresenceHub : Hub
{
    private readonly UserManager<AppUser> _userManager;
    private readonly IHttpContextAccessor _httpContext;
    private readonly PresenceTracker _presenceTracker;
    private readonly ApplicationDbContext _dbContext;

    public PresenceHub(UserManager<AppUser> userManager,IHttpContextAccessor httpContext,PresenceTracker presenceTracker, ApplicationDbContext dbContext)
    {
        _userManager = userManager;
        _httpContext = httpContext;
        _presenceTracker = presenceTracker;
        _dbContext = dbContext;
    }

    public override async Task OnConnectedAsync()
    {
        var userIdClaim =  _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userIdClaim);

        var isOnline = PresenceTracker.ConnectedUser(user.UserName, Context.ConnectionId);
        if (isOnline)
        {
            await Clients.Others.SendAsync("UserIsOnline", user.UserName);
        }

        var currentUsers = await PresenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = await _userManager.FindByIdAsync(userIdClaim);
        var offline =await  _presenceTracker.UserDisConnected(user.UserName, Context.ConnectionId);
        if (offline)
        {
            await Clients.Others.SendAsync("UserIsOffline", user.UserName);
        }

        await base.OnDisconnectedAsync(exception);
    }


}
