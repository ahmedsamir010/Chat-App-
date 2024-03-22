using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
namespace Chat.Application.Features.Accounts.Query.GetUserByName
{
    public class GetUserByNameQuery(string username) : IRequest<MemberDto>
    {
        private readonly string Username = username;

        class Handler(IUserRepository userRepository, IMapper mapper) : IRequestHandler<GetUserByNameQuery,MemberDto>
        {
            private readonly IUserRepository _userRepository = userRepository;
            private readonly IMapper _mapper = mapper;

            public async Task<MemberDto> Handle(GetUserByNameQuery request, CancellationToken cancellationToken)
            {
                if(!string.IsNullOrEmpty(request.Username))
                {
                var user = await _userRepository.GetUserByNameAsync(request.Username);

                    if (user is not null)
                    {
                        var mappedUser= _mapper.Map<MemberDto>(user);
                        return mappedUser;
                    }
                }
                return null;

            }
        }

    }
}
