namespace Chat.Application.Features.Accounts.Query.GetAllBlockedUser
{
    public class GetBlockedUsersQuery : IRequest<IEnumerable<BlockedUsersDto>>
    {
        public class Handler : IRequestHandler<GetBlockedUsersQuery, IEnumerable<BlockedUsersDto>>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;

            public Handler(UserManager<AppUser> userManager,IMapper mapper)
            {
                _userManager = userManager;
                _mapper = mapper;
            }

            public async Task<IEnumerable<BlockedUsersDto>> Handle(GetBlockedUsersQuery request, CancellationToken cancellationToken)
            {
                var blockedUsers = await Task.Run(() => _userManager.Users.Where(x => x.IsBlocked == true));
                var blockedUserDtos = _mapper.Map<IEnumerable<BlockedUsersDto>>(blockedUsers);

                return blockedUserDtos;
            }

        }

    }
}
