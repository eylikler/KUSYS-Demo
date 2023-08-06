using KUSYS_Demo.WebApi.Entities;
using System.Text.Json.Serialization;

namespace KUSYS_Demo.WebApi.Models.Courses
{
    public class CoursesResponse
    {
        public string CourseId { get; set; }
        public string CourseName { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = null;
    }
}
