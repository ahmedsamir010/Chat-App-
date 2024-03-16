using AutoMapper;
using Chat.Application.Features.Accounts.Query.GetAllUsers;
using Chat.Application.Presistance.Contracts;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features.Accounts.Query.GetUserById
{
    public class GetUserByIdQuery : IRequest<MemberDto>
    {
        private readonly string _Id;
        public GetUserByIdQuery(string Id)
        {
            _Id = Id;
        }
        class Handler : IRequestHandler<GetUserByIdQuery, MemberDto>
        {
            private readonly IUserRepository _userRepository;
            private readonly IMapper _mapper;

            public Handler(IUserRepository userRepository, IMapper mapper)
            {
                _userRepository = userRepository;
                _mapper = mapper;
            }
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
