namespace KUSYS_Demo.WebApi.Models.Users;

public record AuthenticateRequest
(
    string Username,
    string Password
);