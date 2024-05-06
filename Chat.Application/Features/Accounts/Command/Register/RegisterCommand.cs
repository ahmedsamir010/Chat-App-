using AutoMapper;
using Chat.Application.Features.Accounts.Validators;
using Chat.Application.Helpers;
using Chat.Application.Models;
using Chat.Application.Presistance.Contracts;
using Chat.Application.Response;
using Chat.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
namespace Chat.Application.Features.Accounts.Command.Register
{
    public class RegisterCommand(RegisterDto registerDto) : IRequest<BaseCommonResponse>
    {
        private readonly RegisterDto _registerDto = registerDto;

        class Hanler : IRequestHandler<RegisterCommand, BaseCommonResponse>
        {
            private readonly UserManager<AppUser> _userManager;
            private readonly IMapper _mapper;
            private readonly ITokenService _tokenService;
            private readonly IConfiguration _configuration;
            public Hanler(UserManager<AppUser> userManager, IMapper mapper, ITokenService tokenService, IConfiguration configuration)
                => (_userManager, _mapper, _tokenService, _configuration) = (userManager, mapper, tokenService, configuration);

            public async Task<BaseCommonResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
            {
                BaseCommonResponse response = new();
                if (!request._registerDto.Email.Contains("@"))
                {
                    response.ResponseStatus = ResponseStatus.BadRequest;
                    response.Message = "Invalid email address.";
                    return response;
                }
                var existingUser = await _userManager.FindByEmailAsync(request._registerDto.Email);
                 Random random= new();

                if (existingUser is not null)
                {
                    response.ResponseStatus = ResponseStatus.BadRequest;
                    response.Message = "User with the same email already exists.";
                    return response;
                }
                 RegisterValidators validator=new();
                var validateResult =await validator.ValidateAsync(request._registerDto);
                if(!validateResult.IsValid) 
                {
                    response.ResponseStatus = ResponseStatus.BadRequest;
                    response.Errors = validateResult.Errors.Select(x => x.ErrorMessage).ToList();
                    return response;
                }
                var user = _mapper.Map<AppUser>(request._registerDto);
                user.VerificationCode = random.Next(1000, 9999).ToString();
                user.UserName = user.FirstName + user.LastName;
                var result = await _userManager.CreateAsync(user, request._registerDto.Password);
                await _userManager.AddToRoleAsync(user, "User");    
                if (result.Succeeded)
                {
                    await SendConfirmationEmail(user);
                    var mainActivePhoto = user.Photos?.FirstOrDefault(u => u.IsMain || u.IsActive);
                    var apiUrlBase = _configuration.GetSection("ApiUrl:Base").Value;
                    response.Data = new
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        PhotoUrl = mainActivePhoto?.Url != null ? apiUrlBase + mainActivePhoto.Url : string.Empty,
                        //Token = _tokenService.CreateAsync(user).Result,
                        Gender=user.Gender
                    };
                    response.ResponseStatus = ResponseStatus.Success;
                }
              
                return response;
               
            }
            private async Task SendConfirmationEmail(AppUser user)
            {
                var emailTitle = "Welcome to Chat Sakr - Confirm Your Email";
                var emailBody = $@"
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>{emailTitle}</title>
</head>
<body style=""font-family: 'Arial', sans-serif; background-color: #f4f4f4; color: #333; padding: 20px; text-align: center;"">
    <div style=""background-color: #fff; border-radius: 10px; padding: 20px; box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);"">
        <h2 style=""color: #007bff;"">Welcome to Chat Sakr, Ahmed! 🎉</h2>
        <p>Thank you for joining Chat Sakr, your go-to platform for seamless communication. We're delighted to have you on board!</p>
        <p style=""background-color: #007bff; color: #fff; padding: 5px; border-radius: 5px;"">Your verification code is: <strong>{user.VerificationCode}</strong></p>
        <p>This simple step ensures the security of your account and enables you to explore all the amazing features Chat Sakr has to offer.</p>
        <p>If you didn't sign up for Chat Sakr, please ignore this email.</p>
        <p>Happy chatting!</p>
        <p>Best regards,<br>Chat Sakr Team</p>
    </div>
    <div style=""margin-top: 20px; font-size: 12px; color: #888;"">
        <p>This is an automated message. Please do not reply to this email.</p>
        <p>&copy; {DateTime.Now.Year} Chat Sakr. All rights reserved.</p>
    </div>
</body>
</html>
";


                var email = new Email
                {
                    Title = emailTitle,
                    Body = emailBody,
                    To = user.Email!
                };

                var emailService = new EmailService();
                await emailService.SendEmailAsync(email);
            }



        }
    }
}
