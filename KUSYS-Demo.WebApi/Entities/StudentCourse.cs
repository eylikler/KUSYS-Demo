namespace KUSYS_Demo.WebApi.Entities;

public class StudentCourse
{
    public int StudentCourseId { get; set; }

    // Foreign keys for Student and Course
    public int StudentId { get; set; }
    public string CourseId { get; set; }

    // Navigation properties for the related entities
    public virtual Student Student { get; set; }
    public virtual Course Course { get; set; }
}
