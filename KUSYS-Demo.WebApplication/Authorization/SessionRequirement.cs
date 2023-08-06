using Microsoft.AspNetCore.Authorization;

namespace KUSYS_Demo.WebApplication.Authorization
{
    public class SessionRequirement : IAuthorizationRequirement
    {
        public SessionRequirement(string sessionHeaderName)
        {
            SessionHeaderName = sessionHeaderName;
        }
        public string SessionHeaderName { get; }
    }
}
