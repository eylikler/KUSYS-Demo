using KUSYS_Demo.WebApplication.DTOs.Courses;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace KUSYS_Demo.WebApplication.DTOs.Students
{
    public class StudentCourseDto
    {
        public int studentCourseId { get; set; }
        public int studentId { get; set; }

        [DisplayName("Ders Kodu")]
        public string courseId { get; set; }

        public StudentDto? student { get; set; }

        public CourseDto? course { get; set; }
    }
}
