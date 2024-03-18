using Chat.Application.Presistance.Contracts;
using MediatR;
using Microsoft.AspNetCore.Http;
namespace Chat.Application.Features.Accounts.Command.UpdatePhoto
{
    public class UpdateFileCommand : IRequest<bool>
    {
        private IFormFile _file { get; set; }

        public UpdateFileCommand(IFormFile File)
        {
            _file = File;
        }
        class Handler : IRequestHandler<UpdateFileCommand, bool>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }

            public async Task<bool> Handle(UpdateFileCommand request, CancellationToken cancellationToken)
            {
                if(request._file is not null)
                {
                    var result=await _userRepository.UploadFile(request._file, "User");
                    if (result is true) return true;
                    else return false;
                }
                return false;
            }
        }




    }
}
