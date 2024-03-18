using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Presistance.Contracts;
using MediatR;
namespace Chat.Application.Features.Accounts.Query.GetUserById
{
    public class GetUserByIdQuery(string Id) : IRequest<MemberDto>
    {
        private readonly string _Id = Id;

        class Handler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUserByIdQuery, MemberDto>
        {
            private readonly IUserRepository _userRepository = userRepository;
            private readonly IMapper _mapper = mapper;

            public async Task<MemberDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
            {
                if (!string.IsNullOrEmpty(request._Id))
                {
                    var user = await _userRepository.GetUserByIdAsync(request._Id);
                    if (user is not null)
                    {
                        var mappedUser = _mapper.Map<MemberDto>(user);
                        return mappedUser;
                    }
                }
                return null;
            }
        }

    }
}
