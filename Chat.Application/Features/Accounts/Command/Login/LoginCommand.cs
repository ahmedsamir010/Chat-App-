namespace Chat.Application.Features.Accounts.Command.Login
{
    public class LoginCommand(LoginDto loginDto) : IRequest<BaseCommonResponse>
    {
        public LoginDto LoginDto { get; set; } = loginDto;
    }

    public class Handler(UserManager<AppUser> userManager,SignInManager<AppUser> signInManager, ITokenService tokenService, IConfiguration configuration, IMapper mapper) : IRequestHandler<LoginCommand, BaseCommonResponse>
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly SignInManager<AppUser> _signInManager = signInManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IConfiguration _configuration = configuration;
        private readonly IMapper _mapper = mapper;

        public async Task<BaseCommonResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            BaseCommonResponse response = new();

            if (!request.LoginDto.Email.Contains("@"))
            {
                response.ResponseStatus = ResponseStatus.BadRequest;
                response.Message = "Invalid email address.";
                return response;
            }

            var user = await _userManager.FindByEmailAsync(request.LoginDto.Email);
                
            if (user is null)
            {
                response.ResponseStatus = ResponseStatus.NotFound;
                response.Message = "User not found. Please check if you have registered with this email.";
                return response;
            }

            if (!await _userManager.IsEmailConfirmedAsync(user))
            {
                response.ResponseStatus = ResponseStatus.NotActivate;
                response.Message = "User is registered but the account is not activated.";
                return response;
            }

            if (_userManager.SupportsUserLockout)
            {
                if (await _userManager.IsLockedOutAsync(user))
                {
                    var lockoutEndDateUtc = await _userManager.GetLockoutEndDateAsync(user);
                    if (lockoutEndDateUtc.HasValue && lockoutEndDateUtc.Value > DateTime.UtcNow)
                    {
                        var lockoutTimeSpan = lockoutEndDateUtc.Value - DateTime.UtcNow;
                        var lockoutDays = Math.Ceiling(lockoutTimeSpan.TotalDays);
                        response.Message = $"Your account is locked out for {lockoutDays} days. Please try again after {lockoutDays} days.";
                        response.ResponseStatus = ResponseStatus.IsBlocked;
                        return response;
                    }
                    await _userManager.ResetAccessFailedCountAsync(user);
                }
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.LoginDto.Password, false);

            if (result.Succeeded)
            {
                // Login successful, potentially map user data using AutoMapper (if applicable)
                var mappedUser = _mapper.Map<UserDto>(user); // Replace UserDto with your desired DTO

                var mainActivePhoto = user.Photos?.FirstOrDefault(u => u.IsMain || u.IsActive);
                var apiUrlBase = _configuration.GetSection("ApiUrl:Base").Value;

                response.ResponseStatus = ResponseStatus.Success;
                response.Message = "Login successful.";
                response.Token = _tokenService.CreateTokenAsync(user).Result;
                return response;
            }

            response.ResponseStatus = ResponseStatus.BadRequest;
            response.Message = "Invalid email or password.";
            return response;
        }
    }
}


