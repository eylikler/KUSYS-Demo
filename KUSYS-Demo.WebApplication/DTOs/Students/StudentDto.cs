using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace KUSYS_Demo.WebApplication.DTOs.Students
{
    public class StudentDto
    {
        public int studentId { get; set; }

        [DisplayName("Adı")]
        [Required(ErrorMessage = "Adı alanı zorunludur.")]
        public string firstName { get; set; }

        [DisplayName("Soyadı")]
        [Required(ErrorMessage = "Soyadı alanı zorunludur.")]
        public string lastName { get; set; }

        [DisplayName("TCKN")]
        [Required(ErrorMessage = "TCKN alanı zorunludur.")]
        public string identityNumber { get; set; }

        [DisplayName("Doğum Tarihi")]
        [Required(ErrorMessage = "Doğum Tarihi alanı zorunludur.")]
        public DateTime birthDate { get; set; }

        [JsonIgnore]
        public virtual ICollection<StudentCourseDto>? StudentCourses { get; set; } = null;
    }
}
