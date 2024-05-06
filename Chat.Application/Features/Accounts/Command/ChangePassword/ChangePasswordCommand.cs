using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Chat.Application.Features.Accounts.Command.ChangePassword
{
    public class ChangePasswordCommand : IRequest<BaseCommonResponse>
    {
        private readonly ChangePasswordDto _changePasswordDto;

        public ChangePasswordCommand(ChangePasswordDto changePasswordDto)
        {
            _changePasswordDto = changePasswordDto;
        }

        class Handler : IRequestHandler<ChangePasswordCommand, BaseCommonResponse>
        {
            private readonly IHttpContextAccessor _httpContext;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IHttpContextAccessor httpContext, UserManager<AppUser> userManager)
            {
                _httpContext = httpContext;
                _userManager = userManager;
            }

            public async Task<BaseCommonResponse> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();
                var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is not null)
                {
                    var userId = userIdClaim.Value;
                    var user = await _userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        if (!await _userManager.IsEmailConfirmedAsync(user))
                        {
                            response.ResponseStatus = ResponseStatus.NotActivate;
                            response.Message = "User is registered but the account is not activated.";
                            return response;
                        }

                        var result = await _userManager.ChangePasswordAsync(user, request._changePasswordDto.CurrentPassword, request._changePasswordDto.NewPassword);

                        if (result.Succeeded)
                        {
                            response.ResponseStatus = ResponseStatus.Success;
                            response.Message = "Password has been changed.";
                            return response;
                        }
                        else
                        {
                            response.ResponseStatus = ResponseStatus.BadRequest;
                            response.Message = "Failed to change the password ... Please Try again";
                            return response;
                           
                        }
                    }
                    else
                    {
                        response.ResponseStatus = ResponseStatus.NotFound;
                        response.Message = "User not found.";
                        return response;
                       
                    }
                }
                else
                {
                    response.ResponseStatus = ResponseStatus.Unauthorized;
                    response.Message = "User claim not found.";
                    return response;
                  
                }
            }
        }
    }
}
