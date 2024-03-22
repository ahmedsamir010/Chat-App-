using AutoMapper;
using Chat.Application.Presistance.Contracts;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
namespace Chat.Application.Features.Accounts.Command.Login
{
    public class LoginCommand(LoginDto loginDto) : IRequest<BaseCommonResponse>
    {
        private readonly LoginDto _loginDto = loginDto;

        class Handler : IRequestHandler<LoginCommand, BaseCommonResponse>
        {

            private readonly UserManager<AppUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly SignInManager<AppUser> _signinmanager;
            private readonly ITokenService _tokenService;
            private readonly IConfiguration _configuration;

            public Handler(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<AppUser> signInManager, ITokenService tokenService,IConfiguration configuration)
                => (_userManager, _roleManager, _signinmanager, _tokenService, _configuration) = (userManager, roleManager, signInManager, tokenService, configuration);
            public async Task<BaseCommonResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();

                var user = await _userManager.Users
                   .Include(p => p.Photos)
                  .FirstOrDefaultAsync(x => x.Email == request._loginDto.Email);
                
                if (user is not null)
                {
                    if (!await _userManager.IsEmailConfirmedAsync(user))
                    {
                        response.responseStatus = ResponseStatus.NotActivate;
                        response.Message = "User is registered but the account is not activated.";
                        return response;
                    }
                    var result = await _signinmanager.CheckPasswordSignInAsync(user, request._loginDto.Password, false);

                    if (result.Succeeded)
                    {
                        var mainActivePhoto = user.Photos?.FirstOrDefault(u => u.IsMain || u.IsActive);
                        var apiUrlBase = _configuration.GetSection("ApiUrl:Base").Value;

                        response.responseStatus = ResponseStatus.Success;
                        response.Message = "Login successful.";
                        response.Data = new
                        {
                            Token = _tokenService.CreateAsync(user).Result,
                            PhotoUrl = mainActivePhoto?.Url != null ? apiUrlBase + mainActivePhoto.Url : string.Empty,
                        };
                        return response;
                    }

                    response.responseStatus = ResponseStatus.BadRequest;
                    response.Message = "Invalid email or password.";
                    return response;
                }

                response.responseStatus = ResponseStatus.NotFound;
                response.Message = "User not found. Please check if you have registered with this email.";
                return response;
            }
        }


    }
}
