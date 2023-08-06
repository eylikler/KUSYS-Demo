using KUSYS_Demo.WebApi.Entities;

namespace KUSYS_Demo.WebApi.Models.Students
{
    public class StudentResponse
    {
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string IdentityNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public virtual ICollection<StudentCourse> StudentCourses { get; set; } = null;
    }
}
