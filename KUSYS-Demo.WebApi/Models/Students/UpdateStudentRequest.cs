using KUSYS_Demo.WebApi.Entities;

namespace KUSYS_Demo.WebApi.Models.Students
{
    public record UpdateStudentRequest
    (
        string FirstName,
        string LastName,
        string IdentityNumber,
        DateTime BirthDate,
        List<StudentCourse>? StudentCourses,
        User? User
    );
}
