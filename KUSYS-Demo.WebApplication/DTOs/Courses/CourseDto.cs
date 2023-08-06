using KUSYS_Demo.WebApplication.DTOs.Students;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KUSYS_Demo.WebApplication.DTOs.Courses
{
    public class CourseDto
    {
        [DisplayName("Ders Kodu")]
        public string courseId { get; set; }

        [DisplayName("Ders Adı")]
        public string courseName { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentCourseDto>? StudentCourses { get; set; } = null;
    }
}
