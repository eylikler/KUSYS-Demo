namespace KUSYS_Demo.WebApi.Entities;

public class Course
{
    public string CourseId { get; set; }
    public string CourseName { get; set; }

    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = null;
}
