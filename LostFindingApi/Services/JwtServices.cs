using LostFindingApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LostFindingApi.Services
{
    public class JwtServices
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> userManager;
        private readonly SymmetricSecurityKey _jwtKey;

        public JwtServices(IConfiguration config,UserManager<User> userManager)
        {
            this._config = config;
            this.userManager = userManager;
            this._jwtKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTKey:Key"]));
        }

        public async Task<string> CreateJWT(User user)
        {
            var userClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.GivenName, user.UserName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Country, user.City),
            };

           
            var roles = await userManager.GetRolesAsync(user);
            
            userClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var Credentails = new SigningCredentials(_jwtKey, SecurityAlgorithms.HmacSha256Signature);
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                SigningCredentials = Credentails,
                Subject = new ClaimsIdentity(userClaims),
                Expires = DateTime.UtcNow.AddMonths(int.Parse(_config["JWTKey:DayExpire"])),
                Issuer = _config["JWTKey:Issuer"],
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(jwt);
        }
    }
}
