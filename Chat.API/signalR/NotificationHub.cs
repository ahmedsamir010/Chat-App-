
using Microsoft.AspNetCore.SignalR;
namespace Chat.API
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string title, string content)
        {
            await Clients.All.SendAsync("ReceiveNotification", title, content);
        }
    }
}
