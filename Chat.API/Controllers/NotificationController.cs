using Chat.Domain.Entities;
using MediatR;
using FirebaseAdmin.Messaging;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Chat.API.Controllers
{
    public class NotificationController : BaseController
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator) : base(mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Send(string token, string title, string content)
        {
            try
            {
                FirebaseApp.Create(new AppOptions
                {
                    Credential = GoogleCredential.FromFile("private_key.json")
                });

                await PushNotification.SendMessage(token, title, content);
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

        public static class PushNotification
        {
            public static async Task SendMessage(string token, string title, string content)
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
                       
                        },
                    
                    };

                    string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                    Console.WriteLine("Successfully sent message: " + response);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}
