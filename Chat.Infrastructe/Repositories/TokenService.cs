using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace Chat.Infrastructe.Repositories
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _configuration;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public TokenService(IConfiguration configuration)
        {

            _configuration = configuration;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]!));
        }

        public async Task<string> CreateAsync(AppUser user)
        {
            return await Task.Run(() =>
            {
                var userId = new Claim(ClaimTypes.NameIdentifier, user.Id);
                var emailClaim = new Claim(JwtRegisteredClaimNames.Email, user.Email!);
                var UserNameClaim = new Claim(ClaimTypes.Name, user.UserName!); ;
                var claims = new List<Claim> { userId, emailClaim, UserNameClaim };

                var creds = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor()
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddDays(2),
                    Issuer = _configuration["Token:Issuer"],
                    SigningCredentials = creds
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                return tokenHandler.WriteToken(token);
            });
        }

    }
}
