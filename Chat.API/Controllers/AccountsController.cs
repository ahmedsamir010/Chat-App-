using Chat.API.Errors;
using Chat.Application.Features.Accounts.Command.Login;
using Chat.Application.Features.Accounts.Command.Register;
using Chat.Application.Features.Accounts.Command.RemoveFile;
using Chat.Application.Features.Accounts.Command.SetMainPhoto;
using Chat.Application.Features.Accounts.Command.UpdateCurrentUser;
using Chat.Application.Features.Accounts.Command.UpdatePhoto;
using Chat.Application.Features.Accounts.Command.VerifyEmai;
using Chat.Application.Features.Accounts.Query.CheckUserNameOrEmail;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Features.Accounts.Query.GetCurrentUser;
using Chat.Application.Features.Accounts.Query.GetUserByEmail;
using Chat.Application.Features.Accounts.Query.GetUserById;
using Chat.Application.Features.Accounts.Query.GetUserByName;
using Chat.Application.Helpers;
using Chat.Application.Helpers.Paginations;
using Chat.Application.Response;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
using System.Text;
namespace Chat.API.Controllers
{
    public class AccountsController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AccountsController> _logger;

        public AccountsController(IMediator mediator, ILogger<AccountsController> logger) : base(mediator)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginDto>> Login(LoginDto loginDto)
        {
            var token = HttpContext.Request.Headers["Authorization"];
            _logger.LogInformation($"Incoming token: {token}");
            var command = new LoginCommand(loginDto);
            var response = await _mediator.Send(command);
            return response.responseStatus switch
            {
                ResponseStatus.Success => Ok(response.Data),
                ResponseStatus.Unauthorized => Unauthorized(response.Data),
                ResponseStatus.NotFound => NotFound(response.Data),
                _ => BadRequest($"Unexpected response status: {response.responseStatus}")
            };
        }
        /// <summary>
        /// Register a new user.
        /// </summary>
        /// <param name="registerDto">Registration details.</param>
        /// <returns>An ActionResult representing the result of the registration.</returns>
        /// <remarks>
        /// Roles:[1=Admin,2=Member,3=User]
        /// //baseUrl+/api/Register
        /// </remarks> 
        [HttpPost("Register")]
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse>> Register([FromBody] RegisterDto registerDto)
        {
            var command = new RegisterCommand(registerDto);
            var response = await _mediator.Send(command);
            return response.responseStatus switch
            {
                ResponseStatus.Success => Ok(response.Data),
                ResponseStatus.BadRequest => new ApiResponse(400,null, response.Errors),
                _ => new ApiResponse(500)
            };
        }
        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        [HttpPost("Verify-Email")]
        public async Task<IActionResult> VerifyEmail(VerificationDto verificationDto)
        {
            var command = new VerifyEmailCommand(verificationDto);
            var response= await _mediator.Send(command);
            if(response is true)
            {
                return Ok("Email Verification");
            }
            return BadRequest("Faild to verify");
        }

        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<ActionResult<ApiResponse>> GetCurrentUser(CancellationToken ct)
        {
            var response = await _mediator.Send(new GetCurrentUserQuery(), ct);
            if (response is null)
            {
                return new ApiResponse(400);
            }
            return Ok(response);
        }
        [HttpGet("Check-User-name-or-Email")]
        public async Task<ActionResult<bool>> CheckUserNameExist([FromQuery] string SearchTerm, CancellationToken ct)
        {
            var result = await _mediator.Send(new CheckUserNameOrEmailQuery(SearchTerm), ct);
            return (result is true) ? true : false;
        }

        [HttpGet("Get-All-Users")]
        public async Task<ActionResult<MemberDto>> GetAllUsers([FromQuery] UserParams userParams,CancellationToken ct)
        {
            var users = await _mediator.Send(new GetUsersQuery(userParams), ct);
            if (users is null)
            {
                return NotFound();
            }
            Response.AddPaginationHeaders(users.CurrentPage,users.TotalPages,users.TotalCount,users.PageSize);
            return Ok(users);
        }
        [HttpGet("Get-User-By-Id")]
        public async Task<ActionResult<MemberDto>> GetUserById(string Id, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(Id), ct);
            if (user is not null) return Ok(user);
            else return NotFound();
        }
        [HttpGet("Get-User-By-Name")]
        public async Task<ActionResult<MemberDto>> GetUserByName(string userName, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserByNameQuery(userName), ct);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpGet("Get-User-By-Email")]
        public async Task<ActionResult<MemberDto>> GetUserByEmail(string email, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(email), ct);
            if (user is null) return NotFound();
            return Ok(user);
        }

        [HttpPut("Update-Current-User")]
        public async Task<ActionResult<ApiResponse>> UpdateCurrentUser(UpdateCurrentUserDto updateCurrentUserDto, CancellationToken ct)
        {
            var command = new UpdateCurrentUserCommand(updateCurrentUserDto);
            var response = await _mediator.Send(command, ct);

            if (response.IsSuccess)
            {
                return Ok(response.Message);
            }
            else
            {
                return new ApiResponse(400, response.Message);
            }
        }

        [HttpPost("Upload-Photo")]
        public async Task<IActionResult> UploadPhoto(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }
            var command = new UpdateFileCommand(file);
            var response = await _mediator.Send(command);
            if (response)
            {
                return Ok("Uploaded successfully");
            }

            return BadRequest("Failed to upload the file");
        }
        

        [HttpDelete("Remove-Photo/{id}")]
        public async Task<IActionResult> RemovePhoto(int id)
        {
            var result = new RemoveCommandPhoto(id);
            var response = await _mediator.Send(result);
            if (response is true)
            {
                return Ok("Remove Successfully");
            }
            return BadRequest("Failed to delete the photo. Please check the provided photo ID and try again.");
        }
        [HttpPut("Set-Main-Photo/{id}")]
        public async Task<IActionResult> SetMainPhoto(int id, CancellationToken ct)
        {
           if(id > 0)
            {
                var command = new SetMainPhotoCommand(id);
                var response = await _mediator.Send(command, ct);

                if (response)
                {
                    return Ok("Main photo set successfully.");
                }
                return BadRequest("Failed to set main photo.");
            }
            else
            {
                return NotFound("The id is not valid");
            }
        }

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
                    string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{model.ClientID}:{model.ClientSecret}"));

                    // Step 2: Set up API Request Header
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

                    // Step 3: Set up Body of Access Token Request
                    var tokenRequest = new Dictionary<string, string>
           {
               { "grant_type", "account_credentials" },
               { "account_id", model.AccountID }
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
