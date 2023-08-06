namespace KUSYS_Demo.WebApi.Authorization;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using KUSYS_Demo.WebApi.Entities;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    private readonly string[] _allowedRoles;

    public AuthorizeAttribute(params string[] allowedRoles)
    {
        _allowedRoles = allowedRoles;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        // skip authorization if action is decorated with [AllowAnonymous] attribute
        var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (allowAnonymous)
            return;

        // authorization
        var user = (User)context.HttpContext.Items["User"];
        if (user == null)
        {
            // Unauthorized
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            return;
        }

        // Check if the user has at least one allowed role
        if (_allowedRoles != null && _allowedRoles.Length > 0)
        {
            var userRoles = context.HttpContext.Items["Roles"] as List<string>;
            if (userRoles == null || !_allowedRoles.Any(role => userRoles.Contains(role)))
            {
                // Forbidden
                context.Result = new JsonResult(new { message = "Forbidden" }) { StatusCode = StatusCodes.Status403Forbidden };
                return;
            }
        }
    }
}