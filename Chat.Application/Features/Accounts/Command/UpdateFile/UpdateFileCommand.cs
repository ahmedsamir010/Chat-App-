namespace Chat.Application.Features.Accounts.Command.UpdateFile
{
    public class UpdateFileCommand(IFormFile File) : IRequest<bool>
    {
        private IFormFile _file { get; set; } = File;

        class Handler : IRequestHandler<UpdateFileCommand, bool>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<bool> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
            {
                if (request._file is not null)
                {
                    var result = await _userRepository.UploadFile(request._file, "User");
                    if (result is true) return true;
                    else return false;
                }
                return false;
            }
        }




    }
}
