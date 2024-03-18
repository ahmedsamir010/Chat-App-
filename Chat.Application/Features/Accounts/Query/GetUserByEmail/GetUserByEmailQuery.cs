using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Presistance.Contracts;
using MediatR;
namespace Chat.Application.Features.Accounts.Query.GetUserByEmail
{
    public class GetUserByEmailQuery : IRequest<MemberDto>
    {
        private readonly string _Email;

        public GetUserByEmailQuery(string email)
        {
            _Email = email;
        }

        class Handler : IRequestHandler<GetUserByEmailQuery, MemberDto>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public Handler(IUserRepository userRepository,IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }
            public async Task<MemberDto> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
            {
                var user = await _userRepository.GetUserByEmail(request._Email);
                if (user is null)
                {
                    return null;
                }
                var mappeduser=_mapper.Map<MemberDto>(user);
                return mappeduser;
            }
        }

    }
}
