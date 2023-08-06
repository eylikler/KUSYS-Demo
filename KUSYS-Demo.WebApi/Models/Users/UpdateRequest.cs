namespace KUSYS_Demo.WebApi.Models.Users;

public record UpdateRequest
(
    string FirstName,
    string LastName,
    string Username,
    string Password
);