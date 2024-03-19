using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationController(IMediator mediator, IHubContext<NotificationHub> hubContext)
        {
            _mediator = mediator;
            _hubContext = hubContext;
        }
        [HttpPost("SendNotificationToMobile")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SendNotificationToMobile(string token, string title, string content)
        {
            try
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });

                await PushNotification.SendToMobile(token, title, content);

                return Ok();
            }
            catch (FirebaseMessagingException ex)
            {
                Console.WriteLine($"FCM Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send notification: Invalid registration token");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send notification: Internal server error");
            }
        }

        [HttpPost("SendNotificationToPC")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> SendNotificationToPC(string title, string content)
        {
            try
            {
                await PushNotification.SendToPC(_hubContext.Clients.All, title, content);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to send notification to PCs: Internal server error");
            }
        }

        public static class PushNotification
        {
            public static async Task SendToMobile(string token, string title, string content)
            {
                try
                {
                    var message = new FirebaseAdmin.Messaging.Message
                    {
                        Data = new Dictionary<string, string>
                        {
                            { "title", title },
                            { "content", content }
                        },
                        Token = token,
                        Notification = new Notification
                        {
                            Title = title,
                            Body = content
                        }
                    };

                    string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    Console.WriteLine("Successfully sent message to mobile: " + response);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error sending message to mobile: " + e.Message);
                    throw;
                }
            }
            public static async Task SendToPC(IClientProxy clients, string title, string content)
            {
                try
                {
                    await clients.SendAsync("ReceiveNotification", title, content);
                    Console.WriteLine("Successfully sent message to PCs via SignalR.");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error sending message to PCs via SignalR: " + e.Message);
                    throw;
                }
            }
        }
    }
}
