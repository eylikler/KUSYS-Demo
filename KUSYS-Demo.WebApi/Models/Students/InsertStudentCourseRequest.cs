using KUSYS_Demo.WebApi.Entities;

namespace KUSYS_Demo.WebApi.Models.Students
{
    public record InsertStudentCourseRequest
    (
        int StudentId,
        string CourseId,
        Student? Student,
        Course? Course
    );
}
