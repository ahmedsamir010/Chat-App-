using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Presistance.Contracts;
using MediatR;
namespace Chat.Application.Features.Accounts.Query.GetUserByName
{
    public class GetUserByNameQuery:IRequest<MemberDto>
    {
        private readonly string Username;
        public GetUserByNameQuery(string username)
        {
            Username = username;
        }
        class handler:IRequestHandler<GetUserByNameQuery,MemberDto>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public handler(IUserRepository userRepository,IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }

            public async Task<MemberDto> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
            {
                if(!string.IsNullOrEmpty(request.Username))
                {
                var user = await _userRepository.GetUserByNameAsync(request.Username);
                    if (user is not null)
                        return _mapper.Map<MemberDto>(user);
                }
                return null;

            }
        }

    }
}
