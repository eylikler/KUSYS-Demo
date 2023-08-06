using AutoMapper;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Models.Students;

namespace KUSYS_Demo.WebApi.Mapper
{
    public class StudentCourseMapper
    {
        private readonly IMapper _mapper;

        public StudentCourseMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InsertStudentCourseRequest, StudentCourse>()
                    .ForMember(dest => dest.Student, opt => opt.Ignore())
                    .ForMember(dest => dest.Course, opt => opt.Ignore());
            });

            _mapper = config.CreateMapper();
        }

        public StudentCourse MapToStudentCourse(InsertStudentCourseRequest model)
        {
            return _mapper.Map<InsertStudentCourseRequest, StudentCourse>(model);
        }
    }
}
