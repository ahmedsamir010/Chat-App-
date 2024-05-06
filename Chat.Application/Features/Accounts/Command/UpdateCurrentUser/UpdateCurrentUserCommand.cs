namespace Chat.Application.Features.Accounts.Command.UpdateCurrentUser
{
    public class UpdateCurrentUserCommand(UpdateCurrentUserDto updateCurrentUserDto) : IRequest<BaseCommonResponse>
    {
        private readonly UpdateCurrentUserDto _updateCurrentUserDto = updateCurrentUserDto;

        class Handler(UserManager<AppUser> userManager, IHttpContextAccessor httpContext, IMapper mapper) : IRequestHandler<UpdateCurrentUserCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager = userManager;
            private readonly IHttpContextAccessor _httpContext = httpContext;
            private readonly IMapper _mapper = mapper;

            public async Task<BaseCommonResponse> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();

                var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is not null)
                {
                    var userId = userIdClaim.Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user is not null)
                    {
                        _mapper.Map(request._updateCurrentUserDto, user);
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            response.Message = "User updated successfully.";
                            response.IsSuccess = true;
                            response.ResponseStatus = ResponseStatus.Success;
                        }
                        else
                        {
                            response.Message = "Failed to update user.";
                            response.Errors = result.Errors.Select(error => error.Description).ToList();
                            response.IsSuccess = false;
                            response.ResponseStatus = ResponseStatus.BadRequest;
                        }
                    }
                    else
                    {
                        response.Message = "User not found.";
                        response.IsSuccess = false;
                        response.ResponseStatus = ResponseStatus.NotFound;
                    }
                }
                else
                {
                    response.Message = "Please make sure you are authenticated.";
                    response.IsSuccess = false;
                    response.ResponseStatus = ResponseStatus.Unauthorized;
                }

                return response;
            }
        }
    }
}
