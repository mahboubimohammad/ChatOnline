using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SharedLibrary.Helpers;
using SharedLibrary.Models.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ChatOnlineApi.Infrastructure.Configuration;

namespace ChatOnlineApi.Infrastructure.Security
{
    public class TokenService
    {
        private readonly JwtOptions _opt;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IOptions<JwtOptions> opt)
        {
            _opt = opt.Value;

            if (string.IsNullOrWhiteSpace(_opt.JwtKey))
                throw new InvalidOperationException("JWT key not configured.");

            _key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_opt.JwtKey)
            );
        }

        public (
            string accessToken,
            string refreshToken
        ) IssueTokens(
            SecurityHelpersDTO.UserTokenClaims tokenClaims,
            string role)
        {
            var now = DateTime.UtcNow;

            var userData = SecurityHelpers.EncryptString(
                JsonConvert.SerializeObject(tokenClaims),
                "B3h!n3H88P@rd@7"
            );

            var claims = new List<Claim>
            {
                new Claim("Token", userData),
                new Claim("Role", role)
            };

            var creds = new SigningCredentials(
                _key,
                SecurityAlgorithms.HmacSha256
            );

            var jwt = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_opt.ExpiresInMinutes),
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler()
                .WriteToken(jwt);

            var refresh = GenerateRefreshToken();

            return (token, refresh);
        }

        private static string GenerateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(48);

            return Convert.ToBase64String(bytes);
        }
    }
}