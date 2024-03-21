using BullPerks.Data.Dto;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace BullPerks.Service
{
    public class AuthService : IAuthService
    {
        private readonly JwtSettings _settings;
        private readonly IConfiguration _configuration;

        public AuthService(IConfiguration configuration)
        {
            _configuration = configuration;
            _settings =  GetJwtSettings();
        }
        public async Task<LoginResponseDto> Authenticate(LoginDto request)
        {
            LoginResponseDto loginResponseDto = new LoginResponseDto();
            if (string.IsNullOrEmpty(request.UserName) || string.IsNullOrEmpty(request.Password))
            {
                loginResponseDto.Success = false;
                loginResponseDto.Message = "Please Enter a Valid UserName / Password";
                return loginResponseDto;
            }

            loginResponseDto.Success = true;
            loginResponseDto.Message = "Login Success";
            loginResponseDto.Token = GenerateJwtToken(request);
            return loginResponseDto;
        }
        public string GenerateJwtToken(LoginDto request)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_settings.Key);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, request.UserName)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Audience = _settings.Audience,
                Issuer = _settings.Issuer,
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public JwtSettings GetJwtSettings()
            {
            JwtSettings settings = new JwtSettings();
            settings.Key = _configuration["JwtSettings:key"]!;
            settings.Audience = _configuration["JwtSettings:audience"]!;
            settings.Issuer = _configuration["JwtSettings:issuer"]!;
            settings.MinutesToExpiration = Convert.ToInt32(_configuration["JwtSettings:minutesToExpiration"]);
            return settings;
        }
    }
}
