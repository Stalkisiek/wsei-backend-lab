using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreApp.Models;
using CoreApp.Repositories;

namespace Infrastucture.Memory;

public class MemoryCourseRepository : MemoryGenericRepository<Course>, ICourseRepository
{
    public MemoryCourseRepository() : base()
    {
        var id1 = Guid.NewGuid();
        var c1 = new Course
        {
            Id = id1,
            Code = "CS101",
            Name = "Wprowadzenie do informatyki",
            EctsCredits = 5,
            CompletionType = CompletionType.Exam,
            AcademicYear = null,
            DegreeProgram = null,
            Enrollments = new List<Student>()
        };
        _data.Add(c1.Id, c1);

        var id2 = Guid.NewGuid();
        var c2 = new Course
        {
            Id = id2,
            Code = "MATH100",
            Name = "Analiza matematyczna",
            EctsCredits = 6,
            CompletionType = CompletionType.Exam,
            AcademicYear = null,
            DegreeProgram = null,
            Enrollments = new List<Student>()
        };
        _data.Add(c2.Id, c2);
    }

    public Task<IEnumerable<Course>> FindByDegreeProgramAsync(Guid degreeProgramId)
    {
        var result = _data.Values.Where(c => c.DegreeProgram != null && c.DegreeProgram.Id == degreeProgramId).ToList();
        return Task.FromResult<IEnumerable<Course>>(result);
    }

    public Task<IEnumerable<Course>> FindByAcademicYearAsync(Guid academicYearId)
    {
        var result = _data.Values.Where(c => c.AcademicYear != null && c.AcademicYear.Id == academicYearId).ToList();
        return Task.FromResult<IEnumerable<Course>>(result);
    }

    public Task<IEnumerable<Course>> FindByLecturerAsync(Guid lecturerId)
    {
        var result = _data.Values.Where(c => c.Enrollments != null && c.Enrollments.Any(s => s.Id == lecturerId)).ToList();
        return Task.FromResult<IEnumerable<Course>>(result);
    }

    public Task<Course?> FindByCodeAsync(string code)
    {
        var result = _data.Values.FirstOrDefault(c => string.Equals(c.Code, code, StringComparison.OrdinalIgnoreCase));
        return Task.FromResult<Course?>(result);
    }
}

