namespace KUSYS_Demo.WebApi.Services;

using AutoMapper;
using BCrypt.Net;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Enums;
using KUSYS_Demo.WebApi.Helpers;
using KUSYS_Demo.WebApi.Mapper;
using KUSYS_Demo.WebApi.Models.Courses;
using KUSYS_Demo.WebApi.Models.Students;
using Microsoft.EntityFrameworkCore;

public interface IStudentService
{
    IEnumerable<Student> GetAll();
    Student GetById(int id);
    void Insert(InsertStudentRequest model);
    void Update(int id, UpdateStudentRequest model);
    void Delete(int id);
    Task<IEnumerable<StudentCoursesResponse>> GetStudentCourseMatchings();
    Task<IEnumerable<StudentCoursesResponse>> GetStudentCourseMatchingsByStudent(int studentId);
    Task<IEnumerable<CoursesResponse>> GetUnmatchedCourses(int studentId);
}

public class StudentService : IStudentService
{
    private DataContext _context;
    private readonly IMapper _mapper;
    private readonly StudentMapper _studentMapper;
    private readonly IHttpContextAccessor _httpContextAccessor;


    public StudentService(
        DataContext context,
        IMapper mapper,
        StudentMapper studentMapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _mapper = mapper;
        _studentMapper = studentMapper;
        _httpContextAccessor = httpContextAccessor;
    }

    private User GetUser()
    {
        return _httpContextAccessor.HttpContext.Items["User"] as User;
    }

    private bool IsUserInRole(User user, string roleName)
    {
        if (user.UserRoles != null)
        {
            return user.UserRoles.Select(ur => ur.Role.Name).Any(role => role.Equals(roleName, StringComparison.OrdinalIgnoreCase));
            //return user.UserRoles.Any(role => role.Role.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));
        }

        return false;
    }

    public void Delete(int id)
    {
        var student = getStudent(id);
        _context.Students.Remove(student);
        _context.SaveChanges();
    }

    

    public IEnumerable<Student> GetAll()
    {
        var authUser = GetUser();

        if (IsUserInRole(authUser, "Admin"))
        {
            return _context.Students;
        }
        else
        {
            return _context.Students.Where(x => x.StudentId == authUser.StudentId);
        }
    }

    public Student? GetById(int id)
    {
        var authUser = GetUser();

        if (IsUserInRole(authUser, "Admin") || id == authUser.StudentId)
        {
            return getStudent(id);
        }        
        else
        {
            return getStudent(authUser.StudentId.Value);
        }
    }

    public async Task<IEnumerable<StudentCoursesResponse>> GetStudentCourseMatchings()
    {
        var studentCourseMatchings = await _context.StudentCourses
            .Include(x => x.Student)
            .Include(x => x.Course)
            .Select(sc => new StudentCoursesResponse
            {
                StudentId = sc.Student.StudentId,
                CourseId = sc.Course.CourseId,
                Student = sc.Student,
                Course = sc.Course,
            })
            .ToListAsync();

        return studentCourseMatchings;
    }

    public async Task<IEnumerable<StudentCoursesResponse>> GetStudentCourseMatchingsByStudent(int studentId)
    {
        List<StudentCoursesResponse> studentCourseMatchings = new List<StudentCoursesResponse>();

        var authUser = GetUser();

        if (IsUserInRole(authUser, "Admin"))
        {
            studentCourseMatchings = await _context.StudentCourses
            .Include(x => x.Student)
            .Include(x => x.Course)
            .Where(x => x.StudentId == studentId)
            .Select(sc => new StudentCoursesResponse
            {
                StudentCourseId = sc.StudentCourseId,
                CourseId = sc.Course.CourseId,
                Course = sc.Course,
                StudentId = sc.StudentId,
                Student = sc.Student
            })
            .ToListAsync();
        }
        else
        {
            studentCourseMatchings = await _context.StudentCourses
            .Include(x => x.Student)
            .Include(x => x.Course)
            .Where(x => x.StudentId == authUser.StudentId)
            .Select(sc => new StudentCoursesResponse
            {
                StudentCourseId = sc.StudentCourseId,
                CourseId = sc.Course.CourseId,
                Course = sc.Course,
                StudentId = sc.StudentId,
                Student = sc.Student
            })
            .ToListAsync();
        }

        return studentCourseMatchings;
    }

    public async Task<IEnumerable<CoursesResponse>> GetUnmatchedCourses(int studentId)
    {
        List<string>? matchedCourseIds = new List<string>();

        var authUser = GetUser();

        if (IsUserInRole(authUser, "Admin"))
        {
            //Daha önce eşleştirilen dersler.
            matchedCourseIds = await _context.StudentCourses
            .Where(x => x.StudentId == studentId)
            .Select(sc => sc.CourseId)
            .ToListAsync();
        }
        else
        {
            //Daha önce eşleştirilen dersler.
            matchedCourseIds = await _context.StudentCourses
            .Where(x => x.StudentId == authUser.StudentId)
            .Select(sc => sc.CourseId)
            .ToListAsync();
        }

        //Tüm derslerden öğrenci ile daha önce eşleştirilen dersleri çıkardık.
        var unmatchedCourses = await _context.Courses
            .Where(course => !matchedCourseIds.Contains(course.CourseId))
            .Select(course => new CoursesResponse
            {
                CourseId = course.CourseId,
                CourseName = course.CourseName
            })
            .ToListAsync();

        return unmatchedCourses;
    }

    public void Insert(InsertStudentRequest model)
    {
        // Validate
        if (_context.Students.Any(x => x.IdentityNumber == model.IdentityNumber))
            throw new AppException("IdentityNumber '" + model.IdentityNumber + "' is already exist");

        var student = _studentMapper.MapToStudent(model);

        // Create User for the Student
        var user = new User
        {
            Username = model.IdentityNumber,
            PasswordHash = BCrypt.HashPassword(model.IdentityNumber),
            IsActive = true
        };

        // Associate User with the Student
        student.User = user;

        // Save student
        _context.Students.Add(student);
        _context.SaveChanges();

        if (_context.Roles.Any(x => x.Name == EnumData.Roles.User.ToString()) == false)
        {
            // Create UserRole for the User
            var role = new Role
            {
                Name = EnumData.Roles.User.ToString()
            };

            // Save Role
            _context.Roles.Add(role);
            _context.SaveChanges();
        }

        var userRoleId = _context.Roles.SingleOrDefault(x => x.Name == EnumData.Roles.User.ToString()).Id;

        // Create UserRole for the User
        var userRole = new UserRole
        {
            UserId = user.Id,
            RoleId = userRoleId
            //RoleId = (int)EnumData.Roles.User
        };

        // Save UserRole
        _context.UserRoles.Add(userRole);
        _context.SaveChanges();
    }

    public void Update(int id, UpdateStudentRequest model)
    {
        var student = getStudent(id);

        // validate
        if (_context.Students.Any(x => x.StudentId != id && x.IdentityNumber == model.IdentityNumber))
            throw new AppException("Another student with the same Identity Number already exists.");

        _studentMapper.MapToStudent(model, student);

        //_context.Students.Update(student);
        _context.SaveChanges();
    }



    // helper methods

    private Student getStudent(int id)
    {
        var student = _context.Students.Find(id);

        if (student == null)
            throw new KeyNotFoundException("Student not found");

        return student;
    }
}