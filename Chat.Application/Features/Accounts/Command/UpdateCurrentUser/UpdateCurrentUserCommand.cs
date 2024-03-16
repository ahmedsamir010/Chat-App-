using AutoMapper;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Application.Features.Accounts.Command.UpdateCurrentUser
{
    public class UpdateCurrentUserCommand : IRequest<BaseCommonResponse>
    {
        private readonly UpdateCurrentUserDto _updateCurrentUserDto;
        public UpdateCurrentUserCommand(UpdateCurrentUserDto updateCurrentUserDto)
        {
            _updateCurrentUserDto = updateCurrentUserDto;
        }

        class Handler : IRequestHandler<UpdateCurrentUserCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IHttpContextAccessor _httpContext;
            private readonly IMapper _mapper;

            public Handler(UserManager<AppUser> userManager,IHttpContextAccessor httpContext,IMapper mapper)
            {
                _userManager = userManager;
                _httpContext = httpContext;
                _mapper = mapper;
            }
            public async Task<BaseCommonResponse> Handle(UpdateCurrentUserCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();

                var userIdClaim = _httpContext?.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);

                if (userIdClaim is not null)
                {
                    var userId = userIdClaim.Value;
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user is not null)
                    {
                        _mapper.Map(request._updateCurrentUserDto, user);
                        var result = await _userManager.UpdateAsync(user);

                        if (result.Succeeded)
                        {
                            response.Message = "User updated successfully.";
                            response.IsSuccess = true;
                            response.responseStatus = ResponseStatus.Success;
                        }
                        else
                        {
                            response.Message = "Failed to update user.";
                            response.Errors = result.Errors.Select(error => error.Description).ToList();
                            response.IsSuccess = false;
                            response.responseStatus = ResponseStatus.BadRequest;
                        }
                    }
                    else
                    {
                        response.Message = "User not found.";
                        response.IsSuccess = false;
                        response.responseStatus = ResponseStatus.NotFound;
                    }
                }
                else
                {
                    response.Message = "Please make sure you are authenticated.";
                    response.IsSuccess = false;
                    response.responseStatus = ResponseStatus.Unauthorized;
                }

                return response;
            }
        }
    }
}
