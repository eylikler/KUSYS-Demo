using KUSYS_Demo.WebApi.Entities;

namespace KUSYS_Demo.WebApi.Models.Students
{
    public class StudentCoursesResponse
    {
        public int StudentCourseId { get; set; }
        public int StudentId { get; set; }
        public string CourseId { get; set; }

        public virtual Student Student { get; set; }
        public virtual Course Course { get; set; }
    }
}
