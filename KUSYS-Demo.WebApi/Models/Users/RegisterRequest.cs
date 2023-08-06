namespace KUSYS_Demo.WebApi.Models.Users;

using KUSYS_Demo.WebApi.Entities;

public record RegisterRequest
(
    string FirstName,
    string LastName,
    string Username,
    string Password,
    List<UserRole> UserRoles,
    Student Student
);