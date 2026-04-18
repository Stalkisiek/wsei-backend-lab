namespace Infrastucture.Repository;

using AutoMapper;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<CoreApp.Models.Student, CoreApp.Dto.StudentSummaryDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId.ToString()))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramName))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<CoreApp.Models.Student, CoreApp.Dto.StudentDetailDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId.ToString()))
            .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.ProgramName)) // Assuming ProgramCode is the same as ProgramName
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramName))
            .ForMember(dest => dest.EnrollmentYear, opt => opt.MapFrom(src => src.EnrollmentYear.ToString()))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.GradePointAverage, opt => opt.Ignore()) // Assuming GPA is calculated separately
            .ForMember(dest => dest.TotalEctsEarned, opt => opt.Ignore()) // Assuming ECTS is calculated separately
            .ForMember(dest => dest.IsEligibleForDiploma, opt => opt.Ignore()); // Assuming eligibility is determined separately

        CreateMap<CoreApp.Dto.StudentCreateDto, CoreApp.Models.Student>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => Guid.Parse(src.StudentId)))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramCode))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => CoreApp.Models.Student
}