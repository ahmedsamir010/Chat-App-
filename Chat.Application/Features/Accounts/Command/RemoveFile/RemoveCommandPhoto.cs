namespace Chat.Application.Features.Accounts.Command.RemoveFile
{
    public class RemoveCommandPhoto(int id) : IRequest<bool>
    {
        private readonly int _Id = id;

        class Handler(IUserRepository userRepository) : IRequestHandler<RemoveCommandPhoto, bool>
        {
            private readonly IUserRepository _userRepository = userRepository;

            public async Task<bool> Handle(RemoveCommandPhoto request, CancellationToken cancellationToken)
            {
                if (request._Id > 0)
                {
                   var response= await _userRepository.RemoveFile(request._Id);
                   if(response)
                    return true;
                }
                return false;
            }
        }
    }
}
