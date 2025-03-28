﻿namespace Chat.API.Controllers
{
    public class AccountsController(IMediator mediator) : BaseController(mediator)
    {
        private readonly IMediator _mediator = mediator;
        /// <summary>
        /// Handles user login with the provided login data.
        /// </summary>
        /// <param name="loginDto">The data required for user login.</param>
        /// <returns>Returns an HTTP response containing the result of the login operation.</returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<LoginDto>> Login(LoginDto loginDto)
        {
            var command = new LoginCommand(loginDto);
            var response = await _mediator.Send(command);
            return response.ResponseStatus switch
            {
                ResponseStatus.Success => Ok(new { Token = response.Token,Message = response.Message }),
                ResponseStatus.BadRequest => BadRequest(response.Message),
                ResponseStatus.IsBlocked => BadRequest(response.Message),
                ResponseStatus.NotActivate => StatusCode(403, new ApiResponse(403, response.Message)),
                _ => NotFound(response.Message)
            };

        }
        /// <summary>
        /// Registers a new user with the provided registration data.
        /// </summary>
        /// <param name="registerDto">The data required for user registration.</param>
        /// <returns>Returns an HTTP response containing the result of the registration operation.</returns>
        [HttpPost("Register")]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterDto>> Register([FromBody] RegisterDto registerDto)
        {
            var command = new RegisterCommand(registerDto);
            var response = await _mediator.Send(command);
            return response.ResponseStatus switch
            {
                ResponseStatus.Success => Ok(response.Data),
                ResponseStatus.BadRequest => BadRequest(response.Errors),
            };
        }

        /// <summary>
        /// Verifies the provided email address using the verification data.
        /// </summary>
        /// <param name="verificationDto">The data containing email address and verification code.</param>
        /// <returns>Returns an indication of whether the email verification was successful or not.</returns>

        [ProducesResponseType(typeof(ApiResponse), 200)]
        [ProducesDefaultResponseType]
        [AllowAnonymous]
        [HttpPost("Verify-Email")]
        public async Task<IActionResult> VerifyEmail(VerificationDto verificationDto)
        {
            var command = new VerifyEmailCommand(verificationDto);
            var response = await _mediator.Send(command);
            if (response is true)
            {
                return Ok("Email Verification");
            }
            return BadRequest("Faild to verify");
        }

        /// <summary>
        /// Changes the password of the authenticated user.
        /// </summary>
        /// <param name="changePasswordDto">The data transfer object containing the current password, new password, and confirm password.</param>
        /// <returns>
        /// An action result containing an ApiResponse indicating the success or failure of the password change operation.
        /// If the password change is successful, returns HTTP 200 with a success message.
        /// If the password change fails, returns HTTP 400 with an error message.
        /// </returns>
        [HttpPost("ChangePassword")]
        public async Task<ActionResult<ApiResponse>> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var command = new ChangePasswordCommand(changePasswordDto);
            var response = await _mediator.Send(command);

            return response.ResponseStatus switch
            {
                ResponseStatus.Success => Ok(new ApiResponse(200, response.Message)),
                ResponseStatus.BadRequest => BadRequest(new ApiResponse(400, response.Message)),
                ResponseStatus.NotActivate => StatusCode(403, new ApiResponse(403, response.Message)),
                ResponseStatus.NotFound => NotFound(new ApiResponse(404, response.Message)),
                ResponseStatus.Unauthorized => StatusCode(401, new ApiResponse(401, response.Message)),
                _ => StatusCode(500, new ApiResponse(500, "Internal Server Error"))
            };
        }
        /// <summary>
        /// Retrieves the details of the currently authenticated user.
        /// </summary>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns an action result containing the details of the currently authenticated user.</returns>

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
        /// <summary>
        /// Checks if a username or email address already exists in the system.
        /// </summary>
        /// <param name="SearchTerm">The username or email address to check for existence.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns a boolean indicating whether the username or email address exists.</returns>

        [HttpGet("Check-User-name-or-Email")]
        public async Task<ActionResult<bool>> CheckUserNameExist([FromQuery] string SearchTerm, CancellationToken ct)
        {
            var result = await _mediator.Send(new CheckUserNameOrEmailQuery(SearchTerm), ct);
            return (result is true) ? true : false;
        }
        /// <summary>
        /// Retrieves a paginated list of all users based on the specified parameters.
        /// </summary>
        /// <param name="userParams">The parameters for pagination and filtering.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns a paginated list of users.</returns>

        [HttpGet("Get-All-Users")]
        public async Task<ActionResult<MemberDto>> GetAllUsers([FromQuery] UserParams userParams, CancellationToken ct)
        {
            var users = await _mediator.Send(new GetUsersQuery(userParams), ct);
            if (users is null)
            {
                return NotFound(new ApiResponse(404, "Not Found Users"));
            }
            Response.AddPaginationHeaders(users.CurrentPage, users.TotalPages, users.TotalCount, users.PageSize);
            return Ok(users);
        }
        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="Id">The unique identifier of the user to retrieve.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns the details of the user with the specified identifier.</returns>

        [HttpGet("Get-User-By-Id")]
        public async Task<ActionResult<MemberDto>> GetUserById(string Id, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserByIdQuery(Id), ct);
            if (user is not null) return Ok(user);
            else return NotFound();
        }
        /// <summary>
        /// Retrieves a user by their username.
        /// </summary>
        /// <param name="userName">The username of the user to retrieve.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns an ActionResult containing the details of the user with the specified username.</returns>
        [HttpGet("Get-User-By-Name")]
        public async Task<ActionResult<MemberDto>> GetUserByName(string userName, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserByNameQuery(userName), ct);
            if (user is null) return NotFound(new ApiResponse(404, "User Not Found"));
            return Ok(user);
        }

        /// <summary>
        /// Retrieves a user by their email address.
        /// </summary>
        /// <param name="email">The email address of the user to retrieve.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns an ActionResult containing the details of the user with the specified email address.</returns>
        [HttpGet("Get-User-By-Email")]
        public async Task<ActionResult<MemberDto>> GetUserByEmail(string email, CancellationToken ct)
        {
            var user = await _mediator.Send(new GetUserByEmailQuery(email), ct);
            if (user is null) return NotFound(new ApiResponse(404, "Email User Not Found"));
            return Ok(user);
        }
        /// <summary>
        /// Updates the current user's information.
        /// </summary>
        /// <param name="updateCurrentUserDto">The data containing updated user information.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns an ActionResult containing the response of the update operation.</returns>
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

        /// <summary>
        /// Uploads a photo file to the server.
        /// </summary>
        /// <param name="file">The photo file to be uploaded.</param>
        /// <returns>Returns an IActionResult representing the result of the upload operation.</returns>
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
        /// <summary>
        /// Deletes a photo with the specified ID.
        /// </summary>
        /// <param name="id">The ID of the photo to be deleted.</param>
        /// <returns>Returns an IActionResult representing the result of the photo deletion operation.</returns>
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
        /// <summary>
        /// Sets the main photo for a resource identified by the given ID.
        /// </summary>
        /// <param name="id">The ID of the resource for which the main photo is to be set.</param>
        /// <param name="ct">Cancellation token to cancel the operation.</param>
        /// <returns>Returns an IActionResult representing the result of setting the main photo.</returns>

        [HttpPut("Set-Main-Photo/{id}")]
        public async Task<IActionResult> SetMainPhoto(int id, CancellationToken ct)
        {
            if (id > 0)
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

        [Authorize(Roles ="Admin")]
        [HttpPost("Block-Profile")]
        public async Task<IActionResult> BlockProfile(BlockProfileDto deleteProfileDto)
        {
            var command = new BlockProfileCommand(deleteProfileDto);
            var response = await _mediator.Send(command);

            if (response.ResponseStatus == ResponseStatus.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }

        [HttpGet("Get-All-Blocked-Users")]
        public async Task<ActionResult<IEnumerable<BlockedUsersDto>>> GetAllBlockedUsers()
        {
            var blockedUsers = await _mediator.Send(new GetBlockedUsersQuery());
            if (blockedUsers == null || !blockedUsers.Any())
            {
                return NotFound(new ApiResponse(404, "No Blocked Users Found"));
            }
            return Ok(blockedUsers);
        }






    }
}
