using AutoMapper;
using Chat.Application.Helpers.Paginations;
using Chat.Application.Presistance.Contracts;
using MediatR;
namespace Chat.Application.Features.Accounts.Query.GetAllUsers
{
    public class GetUsersQuery(UserParams userParams) : IRequest<Pagination<MemberDto>>
    {
        public UserParams _userParams { get; set; } = userParams;

        class Handler(IUserRepository userRepository) : IRequestHandler<GetUsersQuery, Pagination<MemberDto>>
        {
            private readonly IUserRepository _userRepository = userRepository;

            public async Task<Pagination<MemberDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
            {
                var users = await _userRepository.GetMembersAsync(request._userParams);
                return users;
            }
        }
    }
}
