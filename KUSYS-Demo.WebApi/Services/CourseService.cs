using Microsoft.EntityFrameworkCore;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Helpers;



namespace KUSYS_Demo.WebApi.Services
{
    public interface ICourseService
    {
        Task<Course> GetById(string id);
        Task CreateCourseAsync(Course course);
        Task BulkCreateCourseAsync(List<Course> courses);
    }

    public class CourseService : ICourseService
    {
        private DataContext _context;

        public CourseService(DataContext context)
        {
            _context = context;
        }

        public async Task BulkCreateCourseAsync(List<Course> courses)
        {
            // @nuget: Z.EntityFramework.Extensions.EFCore

            await _context.BulkInsertAsync(courses, options => {
                options.InsertIfNotExists = true;
                options.AllowUpdatePrimaryKeys = true;
                options.ColumnPrimaryKeyExpression = courses => courses.CourseId;
            });
        }

        public async Task CreateCourseAsync(Course course)
        {
            // validate
            if (await _context.Courses.AnyAsync(x => x.CourseId == course.CourseId))
                throw new AppException("Course '" + course.CourseId + "' is already exist");

            // save course
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
        }

        public async Task<Course> GetById(string id)
        {
            return await getCourse(id);
        }

        // helper methods

        private async Task<Course> getCourse(string id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) throw new KeyNotFoundException("Course not found");

            return course;
        }
    }
}
