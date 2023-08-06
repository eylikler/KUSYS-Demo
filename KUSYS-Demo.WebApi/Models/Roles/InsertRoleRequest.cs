using System.ComponentModel.DataAnnotations;
using KUSYS_Demo.WebApi.Entities;

namespace KUSYS_Demo.WebApi.Models.Roles
{
    public record InsertRoleRequest
    (
        string Name,
        List<UserRole> UserRoles
    );
}
