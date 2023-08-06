using KUSYS_Demo.WebApplication.DTOs.Students;

namespace KUSYS_Demo.WebApplication.DTOs.Users
{
    public class UserListResponseDto
    {
        public int id { get; set; }
        public int? studentId { get; set; } = null;
        public string? username { get; set; } = null;
        public bool? isActive { get; set; } = null;
        public virtual StudentDto? student { get; set; } = null;
        public virtual List<UserRoleDto>? userRoles { get; set; } = null;
    }
}
