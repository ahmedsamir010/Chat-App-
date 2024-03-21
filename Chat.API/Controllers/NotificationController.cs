
using Google.Apis.Auth.OAuth2;
using Google.Apis.FirebaseCloudMessaging.v1;
using Google.Apis.FirebaseCloudMessaging.v1.Data;
using Google.Apis.Services;
using System.IO;

namespace Chat.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        [HttpPost("SendNotification")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult> SendNotificationToMobile(string token, string title, string content)
        {
            // Call FirebaseHelper method to send notification
            var result = await FirebaseHelper.SendFcmNotificationAsync(token, title, content);

            // Return result
            return Ok(result);
        }
    }

    public class FirebaseHelper
    {
        public async static Task<bool> SendFcmNotificationAsync(string token, string title, string body)
        {
            try
            {
                // Path to Firebase credentials JSON file
                string credentialsPath = "chatnotifications-6273d-firebase-adminsdk-mfaly-5aad4e6df3.json";

                // Initialize Google credential
                GoogleCredential credential;
                using (var stream = new FileStream(credentialsPath, FileMode.Open, FileAccess.Read))
                {
                    credential = GoogleCredential.FromStream(stream);
                }

                // Initialize Firebase Cloud Messaging service
                FirebaseCloudMessagingService service = new FirebaseCloudMessagingService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "chatnotifications"
                });

                // Construct message object
                var message = new Message()
                {
                    Token = token,
                    Notification = new Notification()
                    {
                        Title = title,
                        Body = body
                    }
                };

                // Send message to Firebase API
                var response = await service.Projects.Messages.Send(new SendMessageRequest { Message = message }, "projects/chatnotifications").ExecuteAsync();

                // Check if notification was sent successfully
                return response.Name != null;
            }
            catch (Exception ex)
            {
                // Log exception
                Console.Error.WriteLine($"Failed to send FCM notification: {ex.Message}");
                return false;
            }
        }
    }
}
