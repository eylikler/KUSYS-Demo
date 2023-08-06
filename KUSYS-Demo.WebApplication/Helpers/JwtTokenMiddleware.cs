using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace KUSYS_Demo.WebApplication.Helpers
{
    public class JwtTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            var jwtToken = httpContext.Session.GetString("_JWToken");
            if (!string.IsNullOrEmpty(jwtToken))
            {
                SetUserRolesFromToken(jwtToken, httpContext);
            }

            if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
            {
                var claims = new List<Claim>
                {
                    new Claim("SomeClaim", "SomeValue")
                };

                var appIdentity = new ClaimsIdentity(claims);
                httpContext.User.AddIdentity(appIdentity);
            }

            await _next(httpContext);
        }

        //public async Task InvokeAsync(HttpContext context)
        //{
        //    var jwtToken = context.Session.GetString("_JWToken");
        //    if (!string.IsNullOrEmpty(jwtToken))
        //    {
        //        SetUserRolesFromToken(jwtToken, context);
        //    }

        //    await _next(context);
        //}

        private void SetUserRolesFromToken(string jwtToken, HttpContext context)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadToken(jwtToken) as JwtSecurityToken;

            if (token != null)
            {
                // Rolleri al
                var roles = token.Claims.Where(c => c.Type == "roles").Select(c => c.Value).ToList();
                //var roles = token.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value).ToList();

                // Geçici olarak rolleri sakla
                var identity = new ClaimsIdentity(roles.Select(r => new Claim("roles", r)));
                //var identity = new ClaimsIdentity(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                var principal = new ClaimsPrincipal(identity);
                //context.User = principal;

                context.User.AddIdentity(identity);
            }
        }
    }
}
