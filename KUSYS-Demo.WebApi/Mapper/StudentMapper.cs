using AutoMapper;
using KUSYS_Demo.WebApi.Entities;
using KUSYS_Demo.WebApi.Models.Students;

namespace KUSYS_Demo.WebApi.Mapper
{
    public class StudentMapper
    {
        private readonly IMapper _mapper;

        public StudentMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<InsertStudentRequest, Student>()
                    .ForMember(dest => dest.StudentId, opt => opt.Ignore())
                    .ForMember(dest => dest.User, opt => opt.Ignore())
                    .ForMember(dest => dest.StudentCourses, opt => opt.Ignore());

                cfg.CreateMap<UpdateStudentRequest, Student>()
                    .ForMember(dest => dest.User, opt => opt.Ignore())
                    .ForMember(dest => dest.StudentCourses, opt => opt.Ignore());
            });

            _mapper = config.CreateMapper();
        }

        public Student MapToStudent(InsertStudentRequest model)
        {
            return _mapper.Map<InsertStudentRequest, Student>(model);
        }

        public void MapToStudent(UpdateStudentRequest model, Student student)
        {
            _mapper.Map(model, student);
        }
    }

}
