using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Chat.Application.Features.Accounts.Command.VerifyEmai
{
    public class VerifyEmailCommand(VerificationDto verificationDto) : IRequest<bool>
    {
        private readonly VerificationDto _verificationDto = verificationDto;

        class Handler(UserManager<AppUser> userManager) : IRequestHandler<VerifyEmailCommand, bool>
        {
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<bool> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
            {
                var user = await _userManager.FindByEmailAsync(request._verificationDto.Email);
                if(user is not null)
                {
                        if(user.VerificationCode == request._verificationDto.VerificationCode)
                        {
                            user.EmailConfirmed = true;
                            await _userManager.UpdateAsync(user);
                            user.VerificationCode = string.Empty;
                            await _userManager.UpdateAsync(user);
                            return true;
                    }
                }
                return false;
            }
        }

    }
}
