namespace Infrastucture.Repository;

using AutoMapper;

public class MappingProfile : Profile {
    public MappingProfile() {
        CreateMap<CoreApp.Models.Student, CoreApp.Dto.StudentSummaryDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramName))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

        CreateMap<CoreApp.Models.Student, CoreApp.Dto.StudentDetailDto>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => src.StudentId))
            .ForMember(dest => dest.ProgramCode, opt => opt.MapFrom(src => src.ProgramName)) 
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramName))
            .ForMember(dest => dest.EnrollmentYear, opt => opt.MapFrom(src => src.EnrollmentYear.ToString()))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.GradePointAverage, opt => opt.Ignore()) 
            .ForMember(dest => dest.TotalEctsEarned, opt => opt.Ignore()) 
            .ForMember(dest => dest.IsEligibleForDiploma, opt => opt.Ignore()); 

        CreateMap<CoreApp.Dto.StudentCreateDto, CoreApp.Models.Student>()
            .ForMember(dest => dest.StudentId, opt => opt.MapFrom(src => (src.StudentId ?? string.Empty).Trim()))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramCode))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(_ => CoreApp.Models.StudentStatus.Active));
        CreateMap<CoreApp.Dto.StudentUpdateDto, CoreApp.Models.Student>()
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.YearOfStudy, opt => opt.MapFrom(src => src.YearOfStudy))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.ProgramName, opt => opt.MapFrom(src => src.ProgramCode));
        
        CreateMap<CoreApp.Models.Lecturer, CoreApp.Dto.LecturerSummaryDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Title + " " + src.FirstName + " " + src.LastName));

        CreateMap<CoreApp.Models.Lecturer, CoreApp.Dto.LecturerDetailDto>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Faculty, opt => opt.MapFrom(src => src.Faculty))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<CoreApp.Dto.LecturerCreateDto, CoreApp.Models.Lecturer>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Faculty, opt => opt.MapFrom(src => src.Faculty))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));

        CreateMap<CoreApp.Dto.LecturerUpdateDto, CoreApp.Models.Lecturer>()
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Faculty, opt => opt.MapFrom(src => src.Faculty))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email));
    }
}