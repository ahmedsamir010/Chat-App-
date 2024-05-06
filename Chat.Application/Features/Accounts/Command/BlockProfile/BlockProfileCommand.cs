namespace Chat.Application.Features.Accounts.Command.DeleteProfile
{
    public class BlockProfileCommand(BlockProfileDto blockProfileDto) : IRequest<BaseCommonResponse>
    {
        private readonly BlockProfileDto _blockProfileDto = blockProfileDto;

        class Handler(UserManager<AppUser> userManager) : IRequestHandler<BlockProfileCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager = userManager;

            public async Task<BaseCommonResponse> Handle(BlockProfileCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();

                // Get the user to block
                var userToBlock = await _userManager.FindByIdAsync(request._blockProfileDto.UserId);

                if (userToBlock is null)
                {
                    response.ResponseStatus = ResponseStatus.NotFound;
                    response.Message = "User not found.";
                    return response;
                }

                // Calculate the lockout end date
                var lockoutEndDateUtc = DateTime.UtcNow.AddDays(request._blockProfileDto.BlockDayes);

                // Block the user
                await _userManager.SetLockoutEnabledAsync(userToBlock, true);
                await _userManager.SetLockoutEndDateAsync(userToBlock, lockoutEndDateUtc);
                userToBlock.IsBlocked = true;
                await _userManager.UpdateAsync(userToBlock);
                response.ResponseStatus = ResponseStatus.Success;
                response.Message = $"User with ID {userToBlock.Id} has been blocked for {request._blockProfileDto.BlockDayes} days.";
                return response;
            }
        }
    }

}
