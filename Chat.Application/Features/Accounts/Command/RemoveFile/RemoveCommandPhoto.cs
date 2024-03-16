using Chat.Application.Presistance.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features.Accounts.Command.RemoveFile
{
    public class RemoveCommandPhoto : IRequest<bool>
    {
        private readonly int _Id;

        public RemoveCommandPhoto(int id)
        {
            _Id = id;
        }

        class Handler : IRequestHandler<RemoveCommandPhoto, bool>
        {
            private readonly IUserRepository _userRepository;

            public Handler(IUserRepository userRepository)
            {
                _userRepository = userRepository;
            }
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
