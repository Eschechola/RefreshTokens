using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RefreshTokens.Data.Entities;
using RefreshTokens.Data.Interfaces;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RefreshTokens.Security
{
    public class SecurityService : ISecurityService
    {
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;

        public SecurityService(
            IConfiguration configuration,
            ICacheService cacheService)
        {
            _configuration = configuration;
            _cacheService = cacheService;
        }

        public Token CreateToken(Client client)
        {
            return new Token
            {
                RefreshToken = CreateRefreshToken(client.Id),
                JwtToken = CreateJwtToken(client),
            };
        }

        public string CreateRefreshToken(string clientId)
        {
            var refreshToken = GenerateNewRefreshToken();
            var refreshTokenExpire = DateTime.UtcNow.AddHours(int.Parse(_configuration["RefreshToken:HoursToExpire"]));

            _cacheService.SaveRefreshToken(clientId, refreshToken, refreshTokenExpire);

            return refreshToken;
        }

        public string GenerateNewRefreshToken()
            => Guid.NewGuid().ToString().Replace("-", "");

        public string CreateJwtToken(Client client)
        {
            var claims = new Claim[]
            {
                new Claim("Id", client.Id.ToString()),
                new Claim(ClaimTypes.Name, client.Name),
                new Claim(ClaimTypes.Email, client.Email)
            };

            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:SecretKey"]);

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddHours(int.Parse(_configuration["Jwt:HoursToExpire"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
