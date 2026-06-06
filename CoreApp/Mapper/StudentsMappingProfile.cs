using AutoMapper;
using CoreApp.Models;
using CoreApp.Dto;

namespace CoreApp.Mapper;

public class StudentsMappingProfile : Profile
{
    public StudentsMappingProfile()
    {
        CreateMap<Student, StudentSummaryDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToString()))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.DegreeProgram != null ? src.DegreeProgram.Name : src.ProgramName));

        CreateMap<Student, StudentDetailDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email.ToString()))
            .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.ProgramName))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.DegreeProgram != null ? src.DegreeProgram.Name : src.ProgramName))
            .ForMember(dest => dest.EnrollmentYear, opt => opt.MapFrom(src => src.EnrollmentYear));

        CreateMap<StudentCreateDto, Student>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => (src.StudentId ?? string.Empty).Trim()))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => EmailAddress.From(src.Email)))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramCode))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => StudentStatus.Active));

        CreateMap<StudentUpdateDto, Student>()
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => EmailAddress.From(src.Email)))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramCode));
    }
}

