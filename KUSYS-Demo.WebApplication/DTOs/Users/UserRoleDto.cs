namespace KUSYS_Demo.WebApplication.DTOs.Users
{
    public class UserRoleDto
    {
        public int userId { get; set; }
        public int roleId { get; set; }
        public RoleDto role { get; set; }
    }
}
