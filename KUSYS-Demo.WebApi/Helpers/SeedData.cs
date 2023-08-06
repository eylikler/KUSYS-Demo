using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Models.Roles;
using KUSYS_Demo.WebApi.Services;

namespace KUSYS_Demo.WebApi.Helpers
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IRoleService roleService, IUserService userService, IUserRoleService userRoleService, ICourseService courseService)
        {
            await CreateOrUpdateRole(roleService, "Admin");
            await CreateOrUpdateRole(roleService, "User");

            await CreateOrUpdateUser(roleService, userService, userRoleService, "SysAdmin", "SysAdmin123!", "Admin");

            List<Course> courses = new List<Course>
            {
                new Course { CourseId = "CSI101", CourseName = "Introduction to Computer Science" },
                new Course { CourseId = "CSI102", CourseName = "Algorithms" },
                new Course { CourseId = "MAT101", CourseName = "Calculus" },
                new Course { CourseId = "PHY101", CourseName = "Physics" }
            };

            await BulkCreateOrUpdateCourse(courseService, courses);
        }

        private static async Task CreateOrUpdateRole(IRoleService roleService, string roleName)
        {
            await roleService.CreateRoleAsync(roleName);
        }

        private static async Task CreateOrUpdateUser(IRoleService roleService, IUserService userService, IUserRoleService userRoleService, string userName, string password, string roleName)
        {
            var userRole = new UserRole();

            var role = await roleService.GetRoleByNameAsync(roleName);
            if (role != null)
            {
                userRole.RoleId = role.Id;

                var existingUser = await userService.GetUserByUserNameAsync(userName);
                if (existingUser == null)
                {
                    userRole.UserId = await userService.GenerateUserAsync(userName, password);
                    await userRoleService.GenerateUserRoleAsync(userRole);
                }
                else
                {
                    userRole.UserId = existingUser.Id;
                    await userRoleService.GenerateUserRoleAsync(userRole);
                }
            }
        }

        private static async Task BulkCreateOrUpdateCourse(ICourseService courseService, List<Course> courses)
        {
            await courseService.BulkCreateCourseAsync(courses);
        }
    }
}
