
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Services;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1.Data;
namespace Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("SendNotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult> SendNotificationToMobile(string token, string title, string content)
        {
                
            var result=await FirebaseHelper.SendFcmNotificationAsync(token, title, content,"");
           
            return Ok(result);
        }
        public class FirebaseHelper
        {
            public async static Task<bool> SendFcmNotificationAsync(string token, string? title, string? body, string? url, string? imageUrl = null)
            {
                string credentialsPath = "private_key.json";

                GoogleCredential credential;
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream);
                }
                FirebaseCloudMessagingService service = new(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "chatnotifications"
                });
                var message = new Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body,
                        Image = imageUrl,
                    },
                    Webpush = new WebpushConfig()
                    {
                        FcmOptions = new WebpushFcmOptions()
                        {
                            Link = url
                        }
                    }
                };

                var sendMessageRequest = new SendMessageRequest
                {
                    Message = message,
                };
                try
                {
                    var response = await service.Projects.Messages.Send(sendMessageRequest, "projects/chatnotifications").ExecuteAsync();
                    if (response.Name != null)
                        return true;
                    else
                        return false;
                }
                catch (Exception ex)
                {
                    return false;
                }
            }
        }




    }
}
