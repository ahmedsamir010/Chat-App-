using Chat.Application.Presistance.Contracts;
using Chat.Domain.Entities;
using Microsoft.AspNetCore.Identity;
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
        private readonly UserManager<AppUser> _userManager;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;

        public TokenService(IConfiguration configuration,UserManager<AppUser> userManager)
        {

            _configuration = configuration;
            _userManager = userManager;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Token:Key"]!));
        }

        public async Task<string> CreateTokenAsync(AppUser user)
        {
            var userId = new Claim(ClaimTypes.NameIdentifier, user.Id);
            var emailClaim = new Claim(JwtRegisteredClaimNames.Email, user.Email!);
            var userNameClaim = new Claim(ClaimTypes.Name, user.UserName!);
            var claims = new List<Claim> { userId, emailClaim, userNameClaim };

            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
                claims.Add(new Claim(ClaimTypes.Role, role));
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(2),
                Issuer = _configuration["Token:Issuer"],
                IssuedAt = DateTime.Now,
                SigningCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Token:Audience"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

    }
}
