﻿namespace Chat.Application.Features.Accounts.Command.SetMainPhoto
{
    public class SetMainPhotoCommand(int id) : IRequest<bool>
    {
        private readonly int _id = id;

        class Handler : IRequestHandler<SetMainPhotoCommand, bool>
        {
            private readonly IUserRepository _userRepository;
            private readonly UserManager<AppUser> _userManager;

            public Handler(IUserRepository userRepository , UserManager<AppUser> userManager)
            {
               _userRepository = userRepository;
                _userManager = userManager;
            }
            public async Task<bool> Handle(SetMainPhotoCommand request, CancellationToken cancellationToken)
            {
                if(request._id > 0)
                {
                    var response=await _userRepository.SetMainPhoto(request._id);
                    if (response)
                    {
                        return true;
                    }
                }
                return false;
            }
        }
    }
}
