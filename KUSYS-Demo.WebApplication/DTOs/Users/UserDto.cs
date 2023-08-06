using KUSYS_Demo.WebApplication.DTOs.Students;

namespace KUSYS_Demo.WebApplication.DTOs.Users
{
    public class UserDto
    {
        public int? id { get; set; } = null;
        public int? studentId { get; set; } = null;
        public string? username { get; set; } = null;
        public string? password { get; set; } = null;
        public bool? isActive { get; set; } = null;
        public virtual StudentDto? student { get; set; } = null;
        public virtual List<UserRoleDto>? userRoles { get; set; } = null;
    }
}
