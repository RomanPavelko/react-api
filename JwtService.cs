using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace api
{
    internal static class JwtService
    {
        private const int TokenExpiration = 3600;  //secs = 1 hour
        private static readonly string TokenIssuerName = "React-API";
        private static readonly string TokenIssuerAddress = "https://react-api.azurewebsites.net/";
        private static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5FGE++sf3f==23rfwr3regesf"));
        
        internal static string GenerateTokenForUser(Claim[] claims)
        {
            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = TokenIssuerAddress,
                Issuer = TokenIssuerName,
                Subject = new ClaimsIdentity(claims, "Custom"),
                SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256Signature, SecurityAlgorithms.Sha256Digest),
                Expires = DateTime.UtcNow.AddSeconds(TokenExpiration)
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            return signedAndEncodedToken;
        }

        internal static JwtSecurityToken ValidateDecryptToken(string authToken)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudiences = new[] { TokenIssuerAddress },
                ValidIssuers = new[] { "self", TokenIssuerName },
                IssuerSigningKey = SigningKey
            };
            var tokenHandler = new JwtSecurityTokenHandler();

            SecurityToken validatedToken;

            try
            {
                tokenHandler.ValidateToken(authToken, tokenValidationParameters, out validatedToken);
            }
            catch (Exception)
            {
                return null;
            }

            return validatedToken as JwtSecurityToken;
        }
    }
}