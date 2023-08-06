namespace KUSYS_Demo.WebApi.Entities;

using System.Text.Json.Serialization;

public class Student
{
    public int StudentId { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string IdentityNumber { get; set; }
    public DateTime BirthDate { get; set; }
    [JsonIgnore]
    public virtual ICollection<StudentCourse> StudentCourses { get; set; } = null;
    [JsonIgnore]
    public virtual User User { get; set; } = null;
}
