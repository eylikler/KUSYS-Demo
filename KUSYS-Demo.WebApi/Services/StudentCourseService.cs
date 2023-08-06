using AutoMapper;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Enums;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Mapper;
using KUSYS_Demo.WebApi.Models.Students;

namespace KUSYS_Demo.WebApi.Services
{
    public interface IStudentCourseService
    {
        Task Delete(int id);
        void Insert(InsertStudentCourseRequest model);
    }

    public class StudentCourseService : IStudentCourseService
    {
        private DataContext _context;
        private readonly IMapper _mapper;
        private readonly StudentCourseMapper _studentCourseMapper;

        public StudentCourseService(
            DataContext context,
            StudentCourseMapper studentCourseMapper,
            IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _studentCourseMapper = studentCourseMapper;
        }

        public async Task Delete(int id)
        {
            var course = await getStudentCourse(id);

            _context.StudentCourses.Remove(course);
            await _context.SaveChangesAsync();
        }

        public void Insert(InsertStudentCourseRequest model)
        {
            // Validate
            if (_context.StudentCourses.Any(x => x.StudentId == model.StudentId && x.CourseId == model.CourseId))
                throw new AppException("Student Course is already exist");

            var studentCourse = _studentCourseMapper.MapToStudentCourse(model);

            // Save student
            _context.StudentCourses.Add(studentCourse);
            _context.SaveChanges();
        }

        private async Task<StudentCourse> getStudentCourse(int id)
        {
            var course = await _context.StudentCourses.FindAsync(id);

            if (course == null)
                throw new KeyNotFoundException("Course not found");

            return course;
        }
    }
}
