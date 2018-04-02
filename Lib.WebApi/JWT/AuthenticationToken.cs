using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;
using Microsoft.IdentityModel.Tokens;

namespace Lib.WebApi.JWT
{
    public static class AuthenticationToken
    {
        public static JsonWebToken Generate(SigningConfigurations signingConfigurations, TokenConfigurations tokenConfigurations, string username, int id)
        {
            var identity = new ClaimsIdentity(
                            new GenericIdentity(username, "Login"),
                            new[] {
                                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
                                new Claim(JwtRegisteredClaimNames.UniqueName, username)
                            }
                        );

            DateTime dataCriacao = DateTime.Now;
            DateTime dataExpiracao = dataCriacao + TimeSpan.FromDays(500);

            var handler = new JwtSecurityTokenHandler();
            var securityToken = handler.CreateToken(new SecurityTokenDescriptor
            {
                Issuer = tokenConfigurations.Issuer,
                Audience = tokenConfigurations.Audience,
                SigningCredentials = signingConfigurations.SigningCredentials,
                Subject = identity,
                NotBefore = dataCriacao,
                Expires = dataExpiracao
            });
            var token = handler.WriteToken(securityToken);

            return new JsonWebToken
            {
                Authenticated = true,
                Created = dataCriacao.ToString("yyyy-MM-dd HH:mm:ss"),
                Expiration = dataExpiracao.ToString("yyyy-MM-dd HH:mm:ss"),
                AccessToken = token,
                ID = id,
                Message = "OK"
            };
        }

        public static JwtSecurityToken Decode(string token)
        {
            return new JwtSecurityTokenHandler().ReadJwtToken(token);
        }
    }
}
