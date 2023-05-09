using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;
using ApiERP.Dto;

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

        public Authentication(IConfiguration configuration)
        {
            _configuration = configuration;
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

                var expires = tokenS.ValidTo.ToUniversalTime();
                var now = DateTime.UtcNow;

                if ((expires - now).TotalSeconds < 40) // Refresh the token if it's about to expire in less than 5 minutes
                {
                    // Generate a new token with a new expiration time
                    var newToken = GenerateTokenJwt(jsonString);

                    // Update the response header with the new token
                    context.Response.Headers["refresh-token"] = $"Bearer {newToken}";
                }

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
            var secretKey = _configuration.GetSection("JWT").GetValue<string>("secretKey");
            var audienceToken = _configuration.GetSection("JWT").GetValue<string>("audienceToken");
            var issuerToken = _configuration.GetSection("JWT").GetValue<string>("issuerToken");
            var expireTime = _configuration.GetSection("JWT").GetValue<string>("expireTime");

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
            var secretKey = _configuration.GetSection("JWT").GetValue<string>("secretKey");
            var audienceToken = _configuration.GetSection("JWT").GetValue<string>("audienceToken");
            var issuerToken = _configuration.GetSection("JWT").GetValue<string>("issuerToken");
            var securityKey = new SymmetricSecurityKey(System.Text.Encoding.Default.GetBytes(secretKey));

            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true, //false
                ValidateIssuerSigningKey = true,
                ValidIssuer = issuerToken,
                ValidAudience = audienceToken,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                LifetimeValidator = LifetimeValidator
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
