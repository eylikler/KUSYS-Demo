using AutoMapper;
using KUSYS_Demo.WebApi.Authorization;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Models.Students;
using KUSYS_Demo.WebApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KUSYS_Demo.WebApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class StudentController : ControllerBase
    {
        private IStudentService _studentService;
        private IStudentCourseService _studentCourseService;
        private IMapper _mapper;
        private readonly AppSettings _appSettings;

        public StudentController(
            IStudentService studentService,
            IStudentCourseService studentCourseService,
            IMapper mapper,
            IOptions<AppSettings> appSettings)
        {
            _studentService = studentService;
            _studentCourseService = studentCourseService;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [Authorize("Admin")]
        [HttpPost("insert")]
        public IActionResult Insert(InsertStudentRequest model)
        {
            _studentService.Insert(model);
            return Ok(new { message = "Student Registration successful" });
        }

        [Authorize("Admin")]
        [HttpPut("{id}")]
        public IActionResult Update(int id, UpdateStudentRequest model)
        {
            _studentService.Update(id, model);
            return Ok(new { message = "Student updated successfully" });
        }

        [Authorize("Admin")]
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _studentService.Delete(id);
            return Ok(new { message = "Student deleted successfully" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var students = _studentService.GetAll();

            return Ok(students);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var students = _studentService.GetById(id);
            return Ok(students);
        }

        [HttpGet("courses/{id}")]
        public async Task<IActionResult> GetStudentCourses(int id)
        {
            var studentCourses = await _studentService.GetStudentCourseMatchingsByStudent(id);

            return Ok(studentCourses);
        }

        [HttpGet("unmatchedcourses/{studentId}")]
        public async Task<IActionResult> GetUnmatchedCourses(int studentId)
        {
            var studentCourses = await _studentService.GetUnmatchedCourses(studentId);

            return Ok(studentCourses);
        }

        [HttpDelete("courses/{id}")]
        public async Task<IActionResult> DeleteStudentCourse(int id)
        {
            await _studentCourseService.Delete(id);
            return Ok(new { message = "Student Course deleted successfully" });
        }

        [HttpPost("courses")]
        public IActionResult InsertStudentCourse(InsertStudentCourseRequest model)
        {
            _studentCourseService.Insert(model);
            return Ok(new { message = "Student Course matched successfully" });
        }
    }
}
