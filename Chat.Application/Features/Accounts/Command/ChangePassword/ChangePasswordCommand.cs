using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Chat.Application.Features.Accounts.Command.ChangePassword
{
    public class ChangePasswordCommand(ChangePasswordDto changePasswordDto) : IRequest<bool>
    {
        private readonly ChangePasswordDto _changePasswordDto = changePasswordDto;

        class Handler(IHttpContextAccessor httpContext,UserManager<AppUser> userManager) : IRequestHandler<ChangePasswordCommand, bool>
        {
            private readonly IHttpContextAccessor _httpContext = httpContext;
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
            {
                var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is not null)
                {
                    var userId = userIdClaim.Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    var result = _userManager.ChangePasswordAsync(user!, request._changePasswordDto.CurrentPassword, request._changePasswordDto.NewPassword);
                    if (result.IsCompletedSuccessfully)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }
                else
                {
                    return false;
                }
            }
        }
    }
}
