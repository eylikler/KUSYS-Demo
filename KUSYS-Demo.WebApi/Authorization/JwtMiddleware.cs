namespace KUSYS_Demo.WebApi.Authorization;

using KUSYS_Demo.WebApi.Services;

public class JwtMiddleware
{
    private readonly RequestDelegate _next;

    public JwtMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, IJwtUtils jwtUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = jwtUtils.ValidateToken(token);
        if (userId != null)
        {
            var user = userService.GetById(userId.Value);

            context.Items["User"] = user;

            var roles = user.UserRoles?.Select(ur => ur.Role.Name).ToList();

            // attach roles to context for later use in authorization
            context.Items["Roles"] = roles;
        }

        await _next(context);
    }
}