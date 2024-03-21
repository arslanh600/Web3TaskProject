using BullPerks.Data.Dto;
using BullPerks.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Threading;
namespace BullPerks.Api.ServiceExtension
{
    public static class ServiceExtension
    {
        public static void AddDependencyInjection(this IServiceCollection services)
        {
            services.AddScoped<IAuthService,AuthService>();
            services.AddScoped<IBLPTokenService, BLPTokenService>();

        }
        public static void AddJWTAuthentication(this IServiceCollection services, JwtSettings settings)
        {
            // Register Jwt as the Authentication service
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "JwtBearer";
                options.DefaultChallengeScheme = "JwtBearer";
            })
            .AddJwtBearer("JwtBearer", jwtBearerOptions =>
            {
                jwtBearerOptions.TokenValidationParameters =
              new TokenValidationParameters
              {
                  ValidateIssuerSigningKey = true,
                  IssuerSigningKey = new SymmetricSecurityKey(
                  Encoding.UTF8.GetBytes(settings.Key)),
                  ValidateIssuer = true,
                  ValidIssuer = settings.Issuer,
                  ValidateAudience = true,
                  ValidAudience = settings.Audience,
                  ValidateLifetime = true,
                  ClockSkew = TimeSpan.FromMinutes(settings.MinutesToExpiration)
              };
                jwtBearerOptions.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is JwtSecurityToken accessToken)
                        {
                            //var userName = accessToken.Claims.FirstOrDefault(a => a.Type == Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames.Sub)?.Value;
                            //var email = accessToken.Claims.FirstOrDefault(a => a.Type == "Email")?.Value;
                            //context.HttpContext.Items["Id"] = userName;
                            //var userInfoToken = context.HttpContext.RequestServices.GetRequiredService<UserInfoToken>();
                            //userInfoToken.Id = userName;
                            //userInfoToken.Email = email;
                        }
                        return Task.CompletedTask;
                    }
                };
            });
        }

    }
}
