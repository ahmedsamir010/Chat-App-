using Chat.Application.Features.Message.Command.AddMessage;
using Chat.Application.Features.Message.Query.GetMessageUserRead;
using Chat.Application.Features.Message.Query.GetUserMessages;
using Chat.Application.Helpers.PaginationsMessages;
namespace Chat.API.Controllers
{
    public class MessageController(IMediator mediator) : BaseController(mediator)
    {
        private readonly IMediator _mediator = mediator;

        /// <summary>
        /// Adds a new message.
        /// </summary>
        /// <param name="addMessageDto">The message to add.</param>
        /// <returns>Returns the result of the operation.</returns>
        [HttpPost("AddMessage")]
        public async Task<ActionResult> AddMessage([FromBody] AddMessageDto addMessageDto)
        {
            var command = new AddMessageCommand(addMessageDto);
            var response = await _mediator.Send(command);
            return response.IsSuccess ? Ok(response.Data) : BadRequest(new { response.Message, response.Errors });
        }
        /// <summary>
        /// Retrieves all messages for the current user.
        /// </summary>
        /// <param name="messagesParams">Parameters for filtering, sorting, and pagination.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a paginated list of messages belonging to the current user.</returns>

        [HttpGet("GetUserMessages")]

        public async Task<ActionResult<MessageDto>> GetMessages([FromQuery] UserMessagesParams messagesParams, CancellationToken cancellationToken)
        {
            var messages = await _mediator.Send(new GetUserMessagesCommand(messagesParams), cancellationToken);
            Response.AddPaginationHeaders(messages.CurrentPage, messages.TotalPages, messages.Count, messages.PageSize);
            if (messages is not null)
            {
                return Ok(messages);
            }
            return NotFound("No Message");
        }

        /// <summary>
        /// Retrieves messages read by the specified user.
        /// </summary>
        /// <param name="userName">The username of the recipient whose read messages are to be retrieved.</param>
        /// <param name="ct">Cancellation token.</param>
        /// <returns>Returns a collection of messages read by the specified user.</returns>
        [HttpGet("Get-message-read/{userName}")]
        public async Task<ActionResult<MessageDto>> GetMessageRead(string userName, CancellationToken ct)
        {
            var query = await _mediator.Send(new GetMessageUserReadQuery(userName), ct);
            if (query is not null)
            {
                return Ok(query);
            }
            return NotFound("No Found Read Messages");
        }


        /// <summary>
        /// Creates a Zoom meeting with the provided meeting details.
        /// </summary>
        /// <param name="model">The details of the meeting to be created.</param>
        /// <returns>Returns the URL of the created Zoom meeting.</returns>
        [AllowAnonymous]
        [HttpPost("Create-Meeting")]
        public async Task<IActionResult> CreateMeeting([FromBody] MeetingRequest model)
        {
            try
            {
                var accessToken = await GetZoomAccessToken(model);
                if (string.IsNullOrEmpty(accessToken))
                {
                    return BadRequest("Failed to obtain Zoom access token.");
                }

                var meetingUrl = await CreateZoomMeeting(accessToken, model);
                if (string.IsNullOrEmpty(meetingUrl))
                {
                    return BadRequest("Failed to create Zoom meeting.");
                }

                return Ok(new { MeetingUrl = meetingUrl });
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "Internal server error");
            }
        }
        private async Task<string> GetZoomAccessToken(MeetingRequest model)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    // Step 1: Encode Client ID and Client Secret
                    string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{"B95gfnxAS7Gyx4rAWpiv1A"}:{"psoRfbww33DFHM8lHpg3CB4cBmqQIGZD"}"));

                    // Step 2: Set up API Request Header
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    // Step 3: Set up Body of Access Token Request
                    var tokenRequest = new Dictionary<string, string>
           {
               { "grant_type", "account_credentials" },
               { "account_id", "VNr81cIxSBCPoWzExQVnnQ" }
           };

                    var content = new FormUrlEncodedContent(tokenRequest);

                    // Set 'Content-Type' on the HttpContent
                    content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");

                    // Step 4: Send POST Request to Token API
                    var tokenEndpoint = "https://zoom.us/oauth/token";
                    var tokenResponse = await httpClient.PostAsync(tokenEndpoint, content);

                    // Step 5: Handle Successful Response
                    if (tokenResponse.IsSuccessStatusCode)
                    {
                        var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                        var accessToken = JObject.Parse(tokenContent)["access_token"]!.ToString();
                        Console.WriteLine($"New Access Token obtained: {accessToken}");
                        return accessToken;
                    }
                    else
                    {
                        Console.WriteLine($"Failed to obtain Zoom access token. StatusCode: {tokenResponse.StatusCode}");
                        return "";
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                Console.WriteLine($"Exception in GetZoomAccessToken: {ex.Message}");
                return null;
            }
        }

        private async Task<string> CreateZoomMeeting(string accessToken, MeetingRequest model)
        {
            using (var httpClient = new HttpClient())
            {
                var createMeetingEndpoint = "https://api.zoom.us/v2/users/me/meetings";

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var meetingDetails = new
                {
                    topic = model.Topic,
                    type = 2, // Scheduled meeting
                    start_time = DateTime.Now.AddMinutes(5),
                    password = Guid.NewGuid().ToString().Substring(0, 10)
                };

                var createMeetingResponse = await httpClient.PostAsJsonAsync(createMeetingEndpoint, meetingDetails);
                if (!createMeetingResponse.IsSuccessStatusCode)
                {
                    var errorContent = await createMeetingResponse.Content.ReadAsStringAsync();
                    Console.WriteLine(errorContent); // Log the error response
                    return "";
                }
                // New code to handle timer and meeting termination
                var meetingInfo = await createMeetingResponse.Content.ReadAsStringAsync();
                var meetingId = JObject.Parse(meetingInfo)["id"].ToString();

                // Return join URL
                var joinUrl = JObject.Parse(meetingInfo)["join_url"].ToString();
                var uri = new Uri(joinUrl);
                var queryParameters = System.Web.HttpUtility.ParseQueryString(uri.Query);
                //queryParameters.Remove("pwd");
                var uriBuilder = new UriBuilder(uri)
                {
                    Query = queryParameters.ToString()
                };
                return uriBuilder.Uri.ToString();
            }
        }









    }
}
