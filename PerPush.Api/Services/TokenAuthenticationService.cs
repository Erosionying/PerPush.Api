using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PerPush.Api.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PerPush.Api.Services
{
    public class TokenAuthenticationService : IAuthenticateService
    {
        private readonly IUserService userService;
        private readonly TokenManagement tokenManagement;

        public TokenAuthenticationService(IUserService userService,
            IOptions<TokenManagement> tokenManagement)
        {
            this.userService = userService ?? throw new ArgumentNullException(nameof(userService));
            this.tokenManagement = (tokenManagement.Value) ?? throw new ArgumentNullException(nameof(tokenManagement));
        }
        public bool IsAuthenticated(LoginRequestDto request, out string token)
        {
            token = string.Empty;
            if(!userService.IsValid(request))
            {
                return false;
            }
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, request.UserName)
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenManagement.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var jwtToken = new JwtSecurityToken(tokenManagement.Issuer, tokenManagement.Audience, claims,
                expires: DateTime.Now.AddMinutes(tokenManagement.AccessExpiration),
                signingCredentials: credentials);

            token = new JwtSecurityTokenHandler().WriteToken(jwtToken);

            return true;

        }
    }
}
