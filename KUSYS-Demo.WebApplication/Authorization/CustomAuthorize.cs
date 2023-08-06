using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace KUSYS_Demo.WebApplication.Authorization
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string[] _allowedRoles;

        public CustomAuthorizeAttribute(params string[] allowedRoles)
        {
            _allowedRoles = allowedRoles;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                // Kullanıcı kimlik doğrulaması yapılmamışsa, erişim reddedilir.
                context.Result = new UnauthorizedResult();
                return;
            }

            // Eğer kullanıcı belirtilen rollerden herhangi birine sahipse, erişime izin verilir.
            if (_allowedRoles.Any(role => user.IsInRole(role)))
            {
                return;
            }

            // Eğer kullanıcı "Admin" rolüne sahipse, erişime izin verilir.
            if (user.IsInRole("Admin"))
            {
                return;
            }

            // Diğer durumlarda, erişim reddedilir.
            context.Result = new ForbidResult();
        }
    }
}
