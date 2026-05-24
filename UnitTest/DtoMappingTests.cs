using Xunit;
using AutoMapper;
using Infrastucture.Repository;
using Microsoft.Extensions.Logging.Abstractions;
using CoreApp.Models;
using CoreApp.Dto;

namespace UnitTest;

public class DtoMappingTests
{
    private readonly IMapper _mapper;

    public DtoMappingTests()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile(new MappingProfile()), NullLoggerFactory.Instance);
        _mapper = config.CreateMapper();
    }

    [Fact]
    public void StudentToSummaryAndDetailMappingTest()
    {
        var student = new Student
        {
            StudentId = Guid.NewGuid(),
            FirstName = "Jan",
            LastName = "Kowalski",
            Email = "jan.k@example.com",
            ProgramName = "Informatyka",
            YearOfStudy = 3,
            EnrollmentYear = "2020",
            Status = StudentStatus.Active
        };

        var summary = _mapper.Map<StudentSummaryDto>(student);
        Assert.Equal(student.StudentId.ToString(), summary.StudentId);
        Assert.Equal(student.FirstName, summary.FirstName);
        Assert.Equal(student.LastName, summary.LastName);
        Assert.Equal(student.Email, summary.Email);
        Assert.Equal(student.ProgramName, summary.ProgramName);
        Assert.Equal(student.YearOfStudy, summary.YearOfStudy);
        Assert.Equal(student.Status, summary.Status);

        var detail = _mapper.Map<StudentDetailDto>(student);
        Assert.Equal(student.StudentId.ToString(), detail.StudentId);
        Assert.Equal(student.ProgramName, detail.ProgramCode);
        Assert.Equal(student.ProgramName, detail.ProgramName);
        Assert.Equal(student.EnrollmentYear, detail.EnrollmentYear);
        Assert.Equal(student.YearOfStudy, detail.YearOfStudy);
        Assert.Equal(student.Status, detail.Status);
    }

    [Fact]
    public void StudentCreateDtoToStudentMappingTest()
    {
        var guid = Guid.NewGuid();
        var dto = new StudentCreateDto
        {
            StudentId = guid.ToString(),
            FirstName = "Anna",
            LastName = "Nowak",
            Email = "anna.nowak@example.com",
            ProgramCode = "Informatyka",
            YearOfStudy = 1
        };

        var entity = _mapper.Map<Student>(dto);
        Assert.Equal(guid, entity.StudentId);
        Assert.Equal(dto.FirstName, entity.FirstName);
        Assert.Equal(dto.LastName, entity.LastName);
        Assert.Equal(dto.Email, entity.Email);
        Assert.Equal(dto.ProgramCode, entity.ProgramName);
        Assert.Equal(dto.YearOfStudy, entity.YearOfStudy);
        Assert.Equal(StudentStatus.Active, entity.Status);
    }

    [Fact]
    public void LecturerMappingsTest()
    {
        var lecturer = new Lecturer
        {
            Title = "Prof.",
            FirstName = "Adam",
            LastName = "Smith",
            Email = "adam.smith@example.com",
            Faculty = "Wydzial Informatyki"
        };

        var summary = _mapper.Map<LecturerSummaryDto>(lecturer);
        Assert.Equal(lecturer.Title, summary.Title);
        Assert.Equal(lecturer.Title + " " + lecturer.FirstName + " " + lecturer.LastName, summary.DisplayName);

        var detail = _mapper.Map<LecturerDetailDto>(lecturer);
        Assert.Equal(lecturer.Title, detail.Title);
        Assert.Equal(lecturer.Faculty, detail.Faculty);
        Assert.Equal(lecturer.FirstName, detail.FirstName);
        Assert.Equal(lecturer.LastName, detail.LastName);
        Assert.Equal(lecturer.Email, detail.Email);
    }

    [Fact]
    public void LecturerCreateAndUpdateMappingTest()
    {
        var createDto = new CoreApp.Dto.LecturerCreateDto
        {
            Title = "Dr",
            Faculty = "Wydzial Matematyki",
            FirstName = "Ewa",
            LastName = "Kowal",
            Email = "ewa.k@example.com"
        };

        var lecturer = _mapper.Map<Lecturer>(createDto);
        Assert.Equal(createDto.Title, lecturer.Title);
        Assert.Equal(createDto.Faculty, lecturer.Faculty);
        Assert.Equal(createDto.FirstName, lecturer.FirstName);
        Assert.Equal(createDto.LastName, lecturer.LastName);
        Assert.Equal(createDto.Email, lecturer.Email);

        var updateDto = new CoreApp.Dto.LecturerUpdateDto
        {
            Title = "Prof.",
            Faculty = "Wydzial Fizyki",
            FirstName = "Ewa",
            LastName = "Kowal",
            Email = "ewa.k@example.com"
        };

        var updated = _mapper.Map(updateDto, lecturer);
        Assert.Equal(updateDto.Title, updated.Title);
        Assert.Equal(updateDto.Faculty, updated.Faculty);
    }
}




