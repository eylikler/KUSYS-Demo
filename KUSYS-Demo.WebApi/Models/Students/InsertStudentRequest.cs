using System.ComponentModel.DataAnnotations;
using KUSYS_Demo.WebApi.Entities;

namespace KUSYS_Demo.WebApi.Models.Students
{
    public record InsertStudentRequest
    (        
        string FirstName,    
        string LastName,    
        string IdentityNumber,    
        DateTime BirthDate,
        List<StudentCourse>? StudentCourses,
        User? User
    );
}
