using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using ApiERP.Dto;
using Microsoft.IdentityModel.Tokens;

namespace ApiERP.Middlewares
{
    public class Authentication
    {
        private readonly IConfiguration _configuration;
        private readonly RequestDelegate _next;
        private static DtoUserInfo user;

        public static DtoUserInfo currentUser() => user;

        public Authentication(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public Authentication()
        {
            _configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        }

        public async Task Invoke(HttpContext context)
        {
            string token;

            // existe token
            if (!TryRetrieveToken(context.Request, out token))
            {
            }

            try
            {
                SecurityToken securityToken;
                var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                Thread.CurrentPrincipal = tokenHandler.ValidateToken(token, AuthParameters(), out securityToken);
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                token = token.StartsWith("Bearer ") ? token.Substring(7) : token;
                var tokenS = handler.ReadJwtToken(token);
                string jsonString = tokenS.Claims.FirstOrDefault().Value;

                user = JsonSerializer.Deserialize<DtoUserInfo>(jsonString);

                await _next(context);
            }

            catch (Exception e)
            {
                await _next(context);
            }
        }

        public string GenerateTokenJwt(string username)
        {
            // appsetting for Token JWT
            var secretKey = _configuration.GetSection("Configuration").GetValue<string>("JWT_SECRET_KEY");
            var audienceToken = _configuration.GetSection("Configuration").GetValue<string>("JWT_AUDIENCE_TOKEN");
            var issuerToken = _configuration.GetSection("Configuration").GetValue<string>("JWT_ISSUER_TOKEN");
            var expireTime = _configuration.GetSection("Configuration").GetValue<string>("JWT_EXPIRE_MINUTES");

            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) });

            // create token to the user
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();

            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials
            );

            return tokenHandler.WriteToken(jwtSecurityToken);
        }

        public TokenValidationParameters AuthParameters()
        {
            var secretKey = _configuration.GetSection("Configuration").GetValue<string>("JWT_SECRET_KEY");
            var audienceToken = _configuration.GetSection("Configuration").GetValue<string>("JWT_AUDIENCE_TOKEN");
            var issuerToken = _configuration.GetSection("Configuration").GetValue<string>("JWT_ISSUER_TOKEN");
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuerToken,
                ValidAudience = audienceToken,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
            };
        }

        private static bool TryRetrieveToken(HttpRequest request, out string token)
        {
            token = null;

            Microsoft.Extensions.Primitives.StringValues authzHeaders;

            if (!request.Headers.TryGetValue("Authorization", out authzHeaders) || authzHeaders.Count() > 1)
            {
                return false;
            }
            var bearerToken = authzHeaders.ElementAt(0);
            token = bearerToken.StartsWith("Bearer ") ? bearerToken.Substring(7) : bearerToken;
            return true;
        }

        public bool LifetimeValidator(DateTime? notBefore, DateTime? expires, SecurityToken securityToken, TokenValidationParameters validationParameters)
        {
            if (expires != null)
            {
                if (DateTime.UtcNow < expires) return true;
            }
            return false;
        }
    }
}

